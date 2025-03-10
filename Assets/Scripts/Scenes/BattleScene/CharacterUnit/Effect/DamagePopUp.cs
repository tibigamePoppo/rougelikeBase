using UnityEngine;
using DG.Tweening;

public class DamagePopUp : MonoBehaviour
{
    void Start()
    {
        Vector3 randomNoise = new Vector3(Random.Range(-1,1), 0,0) * 10;
        transform.DOLocalJump(randomNoise,-40,1,0.4f).SetEase(Ease.InOutSine).OnComplete(Destroy);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
