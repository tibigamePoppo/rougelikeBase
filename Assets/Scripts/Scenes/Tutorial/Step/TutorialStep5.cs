using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class TutorialStep5 : TutorialStepBase
{
    [SerializeField] private TutorialPanelView[] _panelsStep;
    [SerializeField] private TutorialPanelView[] _panelsStep2;
    [SerializeField] private Transform _arrowBoss;
    [SerializeField] private GameObject _panelBoss;
    [SerializeField] private Button _bossButton;
    [SerializeField] private RectTransform _content;
    private bool _isClickBoss = false;
    public override void Init()
    {

        _arrowBoss.gameObject.SetActive(false);
        _panelBoss.SetActive(false);
        foreach (var step in _panelsStep)
        {
            step.Init();
        }
        foreach (var step in _panelsStep2)
        {
            step.Init();
        }
        _bossButton.OnClickAsObservable().Subscribe(_ => _isClickBoss = true).AddTo(this);
    }

    public override async UniTask TutorialAcition()
    {
        foreach (var step in _panelsStep)
        {
            await step.ActivePanel();
        }
        _arrowBoss.gameObject.SetActive(true);
        _panelBoss.SetActive(true);
        _bossButton.interactable = true;
        _content.anchoredPosition = new Vector2(0, -1290);
        var arrowBoss = _arrowBoss.DOMoveX(-100, 0.7f).SetRelative().SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        await UniTask.WaitUntil(() => _isClickBoss);

        arrowBoss.Kill();
        _bossButton.interactable = false;
        _bossButton.image.color = Color.gray;
        _panelBoss.SetActive(false);
        _arrowBoss.gameObject.SetActive(false);
        foreach (var step in _panelsStep2)
        {
            await step.ActivePanel();
        }
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        SceneManager.LoadScene("Title");
    }
}
 