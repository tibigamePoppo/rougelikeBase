using Scenes.MainScene.Relic;
using UnityEngine;

[CreateAssetMenu(menuName = "ResourcePool/RelicItemPool")] 
public class RelicItemPool : ScriptableObject
{
    public RelicItemBase[] relicItem;
}
