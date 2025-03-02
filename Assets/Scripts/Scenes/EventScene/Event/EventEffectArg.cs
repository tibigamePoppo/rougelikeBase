using Scenes.MainScene.Player;
using Scenes.MainScene.Relic;

[System.Serializable]
public struct EventEffectArg
{
    public string text;
    public int playerPopularityChange;
    public int playerMoneyChange;
    public SceneName changeScene;
    public UnitData[] units;
    public RelicItemBase relic;
    public EventEffectArg(string text,int playerPopularityChange, int playerMoneyChange, SceneName changeScene, UnitData[] units, RelicItemBase relic)
    {
        this.text = text;
        this.playerPopularityChange = playerPopularityChange;
        this.playerMoneyChange = playerMoneyChange;
        this.changeScene = changeScene;
        this.units = units;
        this.relic = relic;
    }
}