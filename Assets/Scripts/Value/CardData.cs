using UnityEngine;

namespace Scenes.MainScene.Player
{
    [CreateAssetMenu(fileName = "NewCardData", menuName = "ResourceValue/CardData")]
    public class CardData : ScriptableObject
    {
        public Card card;
        public int count;
    }
}