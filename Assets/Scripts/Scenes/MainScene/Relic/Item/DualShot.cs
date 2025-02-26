namespace Scenes.MainScene.Relic.Item
{
    public class DualShot : RelicItemBase
    {
        public override void Init()
        {
            PlayerSingleton.Instance.AddRelicItem(this);
            base.Init();
        }
    }
}