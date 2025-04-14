using UnityEngine;
using Scenes.MainScene.Relic;
using System.Linq;

public class ShopRelicItemView : ShopItemView
{
    [SerializeField] private RelicItemPanelView _relicView;
    private RelicItemBase _relic;

    public void Init(RelicItemBase relic)
    {
        _relic = relic;
        _shopCost = HasShopRelicSalceRelic() ? relic.shopCost / 2 : relic.shopCost;
        _relicView.Init(relic);
        _copyObject = _relicView.icon.gameObject;
        BaseInit();
    }
    public override void Bought()
    {
        _relic.Init();
        base.Bought();
        if(HasShopRelicSalceRelic())
        {
            UpdateRelic();
        }
        _boughtEvent.OnNext(default);
    }

    public override void UpdateText()
    {
        _shopCost = HasShopRelicSalceRelic() ? _relic.shopCost / 2 : _relic.shopCost;
        base.UpdateText();
    }

    private void UpdateRelic()
    {
        var relic = PlayerSingleton.Instance.CurrentRelic.First(c => c.relicItemId == 11);
        relic.OnEffect();
    }

    private bool HasShopRelicSalceRelic()
    {
        if (PlayerSingleton.Instance.CurrentRelic.Select(c => c.relicItemId).Contains(11))
        {
            return PlayerSingleton.Instance.CurrentRelic.First(c => c.relicItemId == 11).isEffect;
        }
        else
        {
            return false;
        }
    }
}
