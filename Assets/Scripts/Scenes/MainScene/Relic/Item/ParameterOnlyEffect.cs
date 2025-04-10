namespace Scenes.MainScene.Relic.Item
{
    public class ParameterOnlyEffect : RelicItemBase
    {
        public override void Init()
        {
            PlayerSingleton.Instance.AddRelicItem(this);
            base.Init();
        }
    }
}