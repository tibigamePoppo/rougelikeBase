namespace Scenes.MainScene.Relic.Item
{
    public class Absorption : RelicItemBase
    {
        public override void Init()
        {
            PlayerSingleton.Instance.AddRelicItem(this);
            base.Init();
        }
    }
}