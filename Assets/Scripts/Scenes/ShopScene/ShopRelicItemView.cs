using UnityEngine;
using Scenes.MainScene.Relic;

public class ShopRelicItemView : ShopItemView
{
    [SerializeField] private RelicItemPanelView _relicView;
    private RelicItemBase _relic;

    public void Init(RelicItemBase relic)
    {
        _relic = relic;
        _shopCost = relic.shopCost;
        _relicView.Init(relic);
        BaseInit();
    }
    public override void Bought()
    {
        PlayerSingleton.Instance.AddRelicItem(_relic);
        base.Bought();
    }
}
