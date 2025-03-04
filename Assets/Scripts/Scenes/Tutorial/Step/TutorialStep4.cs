using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;

public class TutorialStep4 : TutorialStepBase
{
    [SerializeField] private TutorialPanelView[] _panelsStep;
    [SerializeField] private Transform _arrowElite;
    [SerializeField] private GameObject _panelElite;
    [SerializeField] private Button _eliteButton;
    [SerializeField] private RectTransform _content;
    private bool _isClickShop = false;
    public override void Init()
    {

        _arrowElite.gameObject.SetActive(false);
        _panelElite.SetActive(false);
        foreach (var step in _panelsStep)
        {
            step.Init();
        }
        _eliteButton.OnClickAsObservable().Subscribe(_ => _isClickShop = true).AddTo(this);
    }

    public override async UniTask TutorialAcition()
    {
        foreach (var step in _panelsStep)
        {
            await step.ActivePanel();
        }
        _arrowElite.gameObject.SetActive(true);
        _panelElite.SetActive(true);
        _eliteButton.interactable = true;
        _content.anchoredPosition = new Vector2(0, -930);
        var arrowShop = _arrowElite.DOMoveX(-100, 0.7f).SetRelative().SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        await UniTask.WaitUntil(() => _isClickShop);

        arrowShop.Kill();
        _eliteButton.interactable = false;
        _eliteButton.image.color = Color.gray;
        _panelElite.SetActive(false);
        _arrowElite.gameObject.SetActive(false);
    }
}
 