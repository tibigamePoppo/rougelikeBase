using UnityEngine;
using DG.Tweening;

public class DamagePopUp : MonoBehaviour
{
    void Start()
    {
        transform.DOLocalMoveY(-50, 0.25f).SetDelay(0.15f).SetEase(Ease.OutCirc).OnComplete(Destroy);
    }

    public void Destroy()
    {
        Destroy(this.gameObject,0.1f);
    }
}
