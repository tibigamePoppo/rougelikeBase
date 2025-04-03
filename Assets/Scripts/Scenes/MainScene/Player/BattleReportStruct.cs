public struct BattleReportStruct 
{
    public int normalBattle;
    public int eliteBattle;
    public int bossBattle;
    public int eventUnit;
    public int shopUnit;
    public int depth;
    public BattleReportStruct(int normalBattle, int eliteBattle, int bossBattle, int eventUnit, int shopUnit, int depth)
    {
        this.normalBattle = normalBattle;
        this.eliteBattle = eliteBattle;
        this.bossBattle = bossBattle;
        this.eventUnit = eventUnit;
        this.shopUnit = shopUnit;
        this.depth = depth;
    }
}
