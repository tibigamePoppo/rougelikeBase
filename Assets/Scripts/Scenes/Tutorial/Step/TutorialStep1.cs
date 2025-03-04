using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;

public class TutorialStep1 : TutorialStepBase
{
    [SerializeField] private TutorialPanelView[] _panelsStep;
    [SerializeField] private TutorialPanelView[] _panelsStep2;
    [SerializeField] private GameObject _panelsClickStart;
    [SerializeField] private GameObject _panelsClickEvent;
    [SerializeField] private Transform _arrow1;
    [SerializeField] private Transform _arrow2;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _eventButton;
    private bool _isClickEventUnit = false;
    private bool _isClickStartUnit = false;
    public override void Init()
    {
        _panelsClickStart.SetActive(false);
        _panelsClickEvent.SetActive(false);
        _arrow1.gameObject.SetActive(false);
        _arrow2.gameObject.SetActive(false);
        foreach (var step in _panelsStep)
        {
            step.Init();
        }
        foreach (var step in _panelsStep2)
        {
            step.Init();
        }

        _startButton.OnClickAsObservable().Subscribe(_ => _isClickStartUnit = true).AddTo(this);
        _eventButton.OnClickAsObservable().Subscribe(_ => _isClickEventUnit = true).AddTo(this);
    }

    public override async UniTask TutorialAcition()
    {
        foreach (var step in _panelsStep)
        {
            await step.ActivePanel();
        }
        _panelsClickStart.SetActive(true);
        _arrow1.gameObject.SetActive(true);
        var arrowMoveStart = _arrow1.DOMoveX(-100, 0.7f).SetRelative().SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);
        await UniTask.WaitUntil(() => _isClickStartUnit);

        arrowMoveStart.Kill();
        _panelsClickStart.SetActive(false);
        _arrow1.gameObject.SetActive(false);
        _startButton.image.color = Color.gray;
        _startButton.interactable = false;
        foreach (var step in _panelsStep2)
        {
            await step.ActivePanel();
        }
        _panelsClickEvent.SetActive(true);
        _arrow2.gameObject.SetActive(true);
        var arrowMoveEvent = _arrow2.DOMoveX(-100, 0.7f).SetRelative().SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        _eventButton.interactable = true;
        await UniTask.WaitUntil(() => _isClickEventUnit);

        arrowMoveEvent.Kill();
        _eventButton.image.color = Color.gray;
        _eventButton.interactable = false;
        _panelsClickEvent.SetActive(false);
        _arrow2.gameObject.SetActive(false);
    }
}
