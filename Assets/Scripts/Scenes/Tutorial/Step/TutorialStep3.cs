using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;

public class TutorialStep3 : TutorialStepBase
{
    [SerializeField] private TutorialPanelView[] _panelsStep;
    [SerializeField] private Transform _arrowShop;
    [SerializeField] private GameObject _panelShop;
    [SerializeField] private Button _ShopButton;
    [SerializeField] private RectTransform _content;
    private bool _isClickShop = false;
    public override void Init()
    {

        _arrowShop.gameObject.SetActive(false);
        _panelShop.SetActive(false);
        foreach (var step in _panelsStep)
        {
            step.Init();
        }
        _ShopButton.OnClickAsObservable().Subscribe(_ => _isClickShop = true).AddTo(this);
    }

    public override async UniTask TutorialAcition()
    {
        foreach (var step in _panelsStep)
        {
            await step.ActivePanel();
        }
        _arrowShop.gameObject.SetActive(true);
        _panelShop.SetActive(true);
        _ShopButton.interactable = true;
        _content.anchoredPosition = new Vector2(0, - 570);
        var arrowShop = _arrowShop.DOMoveX(-100, 0.7f).SetRelative().SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        await UniTask.WaitUntil(() => _isClickShop);

        _ShopButton.interactable = false;
        _ShopButton.image.color = Color.gray;
        arrowShop.Kill();
        _panelShop.SetActive(false);
        _arrowShop.gameObject.SetActive(false);
    }
}
