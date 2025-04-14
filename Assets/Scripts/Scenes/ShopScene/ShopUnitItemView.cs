using UnityEngine;
using Scenes.MainScene.Cards;
using Scenes.MainScene.Player;
using System.Linq;

public class ShopUnitItemView : ShopItemView
{
    [SerializeField] private CardView _cardView;
    private UnitData _unit;

    public void Init(UnitData unit)
    {
        _unit = unit;
        _shopCost = HasShopUnitSalceRelic() ? unit.shopCost / 2 : unit.shopCost;
        _cardView.Init(_unit.status);
        _copyObject = _cardView.gameObject;
        BaseInit();
    }

    public override void Bought()
    {
        PlayerSingleton.Instance.AddCard(_unit);
        base.Bought();
        if (HasShopUnitSalceRelic())
        {
            UpdateRelic();
        }
        _boughtEvent.OnNext(default);
    }

    public override void UpdateText()
    {
        _shopCost = HasShopUnitSalceRelic() ? _unit.shopCost / 2 : _unit.shopCost;
        base.UpdateText();
    }

    private void UpdateRelic()
    {
        var relic = PlayerSingleton.Instance.CurrentRelic.First(c => c.relicItemId == 10);
        relic.OnEffect();
    }

    private bool HasShopUnitSalceRelic()
    {
        if(PlayerSingleton.Instance.CurrentRelic.Select(c => c.relicItemId).Contains(10))
        {
            return PlayerSingleton.Instance.CurrentRelic.First(c => c.relicItemId == 10).isEffect;
        }
        else
        {
            return false;
        }
    }
}
