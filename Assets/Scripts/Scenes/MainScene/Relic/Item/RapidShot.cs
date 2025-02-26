namespace Scenes.MainScene.Relic.Item
{
    public class RapidShot : RelicItemBase
    {
        public override void Init()
        {
            PlayerSingleton.Instance.AddRelicItem(this);
            base.Init();
        }
    }
}