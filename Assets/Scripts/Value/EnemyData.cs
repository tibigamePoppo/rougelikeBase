using Scenes.MainScene.Player;
using UnityEngine;

[CreateAssetMenu(menuName = "ResourceValue/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string _enemyName;
    public Sprite _enemySprite;
    public int minStageDepth;
    public int maxStageDepth;
    public UnitEnemyGroup[] _unitGroupData = new UnitEnemyGroup[6];
}

[System.Serializable]
public struct UnitEnemyGroup
{
   public UnitData[] _unitData;
}
