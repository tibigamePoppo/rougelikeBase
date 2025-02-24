using Scenes.MainScene.Player;

[System.Serializable]
public struct EventEffectArg
{
    public string text;
    public int playerHpChange;
    public int playerMoneyChange;
    public SceneName changeScene;
    public UnitData[] units;
    public EventEffectArg(string text,int playerHpChange, int playerMoneyChange, SceneName changeScene, UnitData[] units)
    {
        this.text = text;
        this.playerHpChange = playerHpChange;
        this.playerMoneyChange = playerMoneyChange;
        this.changeScene = changeScene;
        this.units = units;
    }
}