using System.Collections.Generic;
using Scenes.MainScene.Player;
using UnityEngine;

[CreateAssetMenu(menuName = "ResourcePool/CardPool")]
public class CardPool : ScriptableObject
{
    public List<CardData> cards;
}
