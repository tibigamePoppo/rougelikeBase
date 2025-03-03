using Scenes.MainScene.Player;
using UnityEngine;

[CreateAssetMenu(menuName = "ResourceValue/UnitUpgrade")]
public class UnitUpgradeInfo : ScriptableObject
{
    public UnitData baseUnit;
    public UnitData[] UpgradeUnit;
}