namespace Scenes.MainScene.Relic.Item
{
    public class InitialShield : RelicItemBase
    {
        public override void Init()
        {
            PlayerSingleton.Instance.AddRelicItem(this);
            base.Init();
        }
    }
}