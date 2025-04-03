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
        _copyObject = _relicView.icon.gameObject;
        BaseInit();
    }
    public override void Bought()
    {
        _relic.Init();
        base.Bought();
    }
}
