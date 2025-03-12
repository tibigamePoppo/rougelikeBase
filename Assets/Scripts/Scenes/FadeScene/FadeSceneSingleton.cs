using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class FadeSceneSingleton : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    public static FadeSceneSingleton Instance;

    void Start()
    {
        Instance = this;
    }

    public void FadeInOutEffect(Action callback = null, float duration = 0.5f,Ease ease = Ease.InOutSine)
    {
       _fadeImage.DOColor(Color.black, duration).SetLoops(2, LoopType.Yoyo).SetEase(ease).OnComplete(() => callback?.Invoke());
    }

    public void FadeInEffet(Action callback = null, float duration = 0.2f, Ease ease = Ease.InOutSine)
    {
        _fadeImage.DOColor(Color.black, duration).SetEase(ease).OnComplete(() => callback?.Invoke());
    }

    public void FadeOutEffet(Action callback = null,float duration = 0.2f, Ease ease = Ease.InOutSine)
    {
        _fadeImage.DOColor(Color.clear, duration).SetEase(ease).OnComplete(() => callback?.Invoke());
    }

}
