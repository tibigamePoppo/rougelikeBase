using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;

public class TutorialStep2 : TutorialStepBase
{
    [SerializeField] private TutorialPanelView[] _panelsStep;
    [SerializeField] private TutorialPanelView[] _panelsStep2;
    [SerializeField] private Transform _arrowReturn;
    [SerializeField] private Transform _arrowBattle;
    [SerializeField] private GameObject _eventSceneElement;
    [SerializeField] private GameObject _panelBattle;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _battleButton;
    [SerializeField] private RectTransform _content;
    private bool _isClickBack = false;
    private bool _isClickBattleUnit = false;
    public override void Init()
    {
        _arrowReturn.gameObject.SetActive(false);
        _arrowBattle.gameObject.SetActive(false);
        _eventSceneElement.SetActive(false);
        _panelBattle.SetActive(false);
        foreach (var step in _panelsStep)
        {
            step.Init();
        }
        foreach (var step in _panelsStep2)
        {
            step.Init();
        }
        _backButton.OnClickAsObservable().Subscribe(_ => _isClickBack = true).AddTo(this);
        _battleButton.OnClickAsObservable().Subscribe(_ => _isClickBattleUnit = true).AddTo(this);
    }

    public override async UniTask TutorialAcition()
    {
        _eventSceneElement.SetActive(true);
        foreach (var step in _panelsStep)
        {
            await step.ActivePanel();
        }
        _arrowReturn.gameObject.SetActive(true);
        var arrowReturn = _arrowReturn.DOMoveX(-100, 0.7f).SetRelative().SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        await UniTask.WaitUntil(() => _isClickBack);

        arrowReturn.Kill();
        _eventSceneElement.SetActive(false);
        _arrowReturn.gameObject.SetActive(false);

        foreach (var step in _panelsStep2)
        {
            await step.ActivePanel();
        }
        _battleButton.interactable = true;
        _panelBattle.SetActive(true);
        _arrowBattle.gameObject.SetActive(true);
        _content.anchoredPosition = new Vector2(0,-220);
        var arrowBattle = _arrowBattle.DOMoveX(-100, 0.7f).SetRelative().SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        await UniTask.WaitUntil(() => _isClickBattleUnit);
        arrowBattle.Kill();
        _panelBattle.SetActive(false);
        _arrowBattle.gameObject.SetActive(false);
        _battleButton.image.color = Color.gray;
        _battleButton.interactable = false;

    }
}
