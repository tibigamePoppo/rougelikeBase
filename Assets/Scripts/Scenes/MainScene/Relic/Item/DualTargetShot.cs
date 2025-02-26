namespace Scenes.MainScene.Relic.Item
{
    public class DualTargetShot : RelicItemBase
    {
        public override void Init()
        {
            PlayerSingleton.Instance.AddRelicItem(this);
            base.Init();
        }
    }
}