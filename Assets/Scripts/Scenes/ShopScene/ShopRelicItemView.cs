using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Scenes.MainScene.Cards;
using UniRx;
using Scenes.MainScene.Relic;

public class ShopRelicItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private RelicItemPanelView _relicView;
    [SerializeField] private Image _soldImage;
    private bool _isBought = false;

    public void Init(RelicItemBase relic)
    {
        _soldImage.gameObject.SetActive(false);
        _costText.text = $"{relic.shopCost} G";
        _relicView.Init(relic);

        Button button = GetComponent<Button>();
        button.OnClickAsObservable().Subscribe(_ =>
        {
            if (PlayerSingleton.Instance.CurrentMoney >= relic.shopCost && !_isBought)
            {
                PlayerSingleton.Instance.AddRelicItem(relic);
                PlayerSingleton.Instance.ChangeMoney(-relic.shopCost);
                _isBought = true;
                _soldImage.gameObject.SetActive(true);
            }
        }).AddTo(this);
    }
}
