using UnityEngine;
using Scenes.MainScene.Cards;
using Scenes.MainScene.Player;

public class ShopUnitItemView : ShopItemView
{
    [SerializeField] private CardView _cardView;
    private UnitData _unit;

    public void Init(UnitData unit)
    {
        _unit = unit;
        _shopCost = _unit.shopCost;
        _cardView.Init(_unit.status);
        _copyObject = _cardView.gameObject;
        BaseInit();
    }

    public override void Bought()
    {
        PlayerSingleton.Instance.AddCard(_unit);
        base.Bought();
    }
}
