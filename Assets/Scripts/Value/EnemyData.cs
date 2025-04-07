using Scenes.MainScene.Player;
using UnityEngine;

[CreateAssetMenu(menuName = "ResourceValue/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int id;
    public string _enemyName;
    public Sprite _enemySprite;
    public int minStageDepth;
    public int maxStageDepth;
    public UnitEnemyGroup[] unitGroupData = new UnitEnemyGroup[6];
}

[System.Serializable]
public struct UnitEnemyGroup
{
   public UnitData[] unitData;
}
