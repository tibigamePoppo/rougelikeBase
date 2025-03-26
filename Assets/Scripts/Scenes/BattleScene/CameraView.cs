using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CameraView : MonoBehaviour
{
    [SerializeField] private Camera _battleCamera;
    private float _moveSpeed = 15f;
    private Vector3 moveDirection;
    private float edgeThreshold = 0.01f; // 画面端と判定する割合（5%）
    private const float MAXCAMERAVIEW = 25;
    private const float MINCAMERAVIEW = 10;

    private const float MAXY = 25;
    private const float MINY = -40;
    private const float MAXX = 25;
    private const float MINX = -25;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.W)).Subscribe(_ => UpdateTransfrom(Vector3.forward));
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.A)).Subscribe(_ => UpdateTransfrom(Vector3.left));
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.S)).Subscribe(_ => UpdateTransfrom(Vector3.back));
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.D)).Subscribe(_ => UpdateTransfrom(Vector3.right));
        this.UpdateAsObservable().Subscribe(_ =>
        {
            Vector3 mousePos = Input.mousePosition;
            moveDirection = Vector3.zero;

            if (mousePos.x < screenWidth * edgeThreshold)  // 左端
                moveDirection.x = -1;
            if (mousePos.x > screenWidth * (1 - edgeThreshold))  // 右端
                moveDirection.x = 1;
            if (mousePos.y < screenHeight * edgeThreshold)  // 下端
                moveDirection.z = -1;
            if (mousePos.y > screenHeight * (1 - edgeThreshold))  // 上端
                moveDirection.z = 1;

            UpdateTransfrom(moveDirection);
        }).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.DownArrow) && _battleCamera.orthographicSize >= MINCAMERAVIEW)
            .Subscribe(_ => _battleCamera.orthographicSize -= 5f * Time.deltaTime).AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.UpArrow) && _battleCamera.orthographicSize <= MAXCAMERAVIEW)
            .Subscribe(_ => _battleCamera.orthographicSize += 5f * Time.deltaTime).AddTo(this);
    }

    private void UpdateTransfrom(Vector3 vector)
    {
        if ((vector.x > 0 && transform.position.x >= MAXX) || (vector.x < 0 && transform.position.x <= MINX)) vector.x = 0;
        if ((vector.z > 0 && transform.position.z >= MAXY) || (vector.z < 0 && transform.position.z <= MINY)) vector.z = 0;
        transform.position += vector * _moveSpeed * Time.deltaTime;
    }

}
