namespace Scenes.MainScene.Relic.Item
{
    public class HealPosition : RelicItemBase
    {
        public override void Init()
        {
            PlayerSingleton.Instance.AddRelicItem(this);
            base.Init();
        }
    }
}