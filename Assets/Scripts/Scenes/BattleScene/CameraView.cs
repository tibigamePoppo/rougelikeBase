using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        Init();
    }

    private void Init()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.W)).Subscribe(_ => transform.position += new Vector3(0, 0, 1) * _moveSpeed * Time.deltaTime);
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.A)).Subscribe(_ => transform.position += new Vector3(-1, 0, 0) * _moveSpeed * Time.deltaTime);
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.S)).Subscribe(_ => transform.position += new Vector3(0, 0, -1) * _moveSpeed * Time.deltaTime);
        this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.D)).Subscribe(_ => transform.position += new Vector3(1, 0, 0) * _moveSpeed * Time.deltaTime);
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
            transform.position += moveDirection * _moveSpeed * Time.deltaTime;
        }).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.DownArrow) && _battleCamera.orthographicSize >= MINCAMERAVIEW)
            .Subscribe(_ => _battleCamera.orthographicSize -= 5f * Time.deltaTime).AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.UpArrow) && _battleCamera.orthographicSize <= MAXCAMERAVIEW)
            .Subscribe(_ => _battleCamera.orthographicSize += 5f * Time.deltaTime).AddTo(this);
    }
}
