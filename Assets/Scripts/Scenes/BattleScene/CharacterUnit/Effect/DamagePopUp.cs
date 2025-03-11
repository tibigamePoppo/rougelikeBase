using UnityEngine;
using DG.Tweening;

public class DamagePopUp : MonoBehaviour
{
    void Start()
    {
        //transform.DOLocalJump(transform.position,-40,1,0.4f).SetEase(Ease.InOutSine).OnComplete(Destroy);
        transform.DOLocalMoveY(-50, 0.25f).SetDelay(0.15f).SetEase(Ease.OutSine).OnComplete(Destroy);
    }

    public void Destroy()
    {
        Destroy(this.gameObject,0.1f);
    }
}
