using Scenes.MainScene.Player;
using UnityEngine;

[CreateAssetMenu(menuName = "ResourceValue/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string _enemyName;
    public Sprite _enemySprite;
    public UnitData[] _unitData;
}
