using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Scenes.MainScene.Cards;
using Scenes.MainScene.Player;
using UniRx;
using UniRx.Triggers;

public class ShopItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private CardView _cardView;
    [SerializeField] private Image _soldImage;
    private bool _isBought = false;

    public void Init(UnitData unit)
    {
        _soldImage.gameObject.SetActive(false);
        _costText.text = $"{unit.shopCost} G";
        _cardView.Init(unit.status);

        Button button = GetComponent<Button>();
        button.OnClickAsObservable().Subscribe(_ =>
        {
            if(PlayerSingleton.Instance.CurrentMoney >= unit.shopCost && !_isBought)
            {
                PlayerSingleton.Instance.AddCard(unit);
                PlayerSingleton.Instance.ChangeMoney(-unit.shopCost);
                _isBought = true;
                _soldImage.gameObject.SetActive(true);
            }
        }).AddTo(this);
    }
}
