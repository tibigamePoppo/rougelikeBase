namespace Scenes.MainScene.Relic.Item
{
    public class AutoHeal : RelicItemBase
    {
        public override void Init()
        {
            PlayerSingleton.Instance.AddRelicItem(this);
            base.Init();
        }
    }
}