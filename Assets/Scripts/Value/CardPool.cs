using System.Collections.Generic;
using System.Linq;
using Scenes.MainScene.Player;
using UnityEngine;

[CreateAssetMenu(menuName = "ResourcePool/CardPool")]
public class CardPool : ScriptableObject
{
    public List<UnitData> cards;
    public List<UnitData> CardList()
    {
        List<UnitData> copyCards = new List<UnitData>();
        foreach (var card in cards)
        {
            copyCards.Add(card);
        }
        return copyCards;
    }
}
