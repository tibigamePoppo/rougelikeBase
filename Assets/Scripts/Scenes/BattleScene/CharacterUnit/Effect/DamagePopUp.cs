using UnityEngine;
using DG.Tweening;

public class DamagePopUp : MonoBehaviour
{
    private const float ANIMATIONTIME = 0.5f;
    void Start()
    {
        transform.DOLocalJump(Vector3.zero, -40, 1, ANIMATIONTIME).SetEase(Ease.InOutSine);
        transform.DOScale(transform.localScale * 1.1f, ANIMATIONTIME / 2).SetEase(Ease.InSine).SetLoops(2, LoopType.Yoyo);
        Destroy(this.gameObject, ANIMATIONTIME + 0.1f);
        //transform.DOLocalMoveY(-50, 0.25f).SetDelay(0.15f).SetEase(Ease.OutCirc).OnComplete(Destroy);
    }
}
