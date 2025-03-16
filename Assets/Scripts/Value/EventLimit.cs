using Scenes.MainScene.Player;
using Scenes.MainScene.Relic;

[System.Serializable]
public struct EventLimit
{
    public int upperLimitMoney;
    public int underLimitMoney;
    public int upperLimitPopularity;
    public int underLimitPopularity;
    public UnitData[] containUnits;
    public UnitData[] uncontainUnits;
    public RelicItemBase[] containRelic;
    public RelicItemBase[] uncontainRelic;
    public string[] passEvent;
    public string[] notPassEvent;
}
