using UnityEngine;

namespace Scenes.MainScene.Player
{
    [System.Serializable]
    public struct Card
    {
        public string name;
        public Sprite sprite;
        public string text;
        public Card(string name, Sprite sprite, string text)
        {
            this.name = name;
            this.sprite = sprite;
            this.text = text;
        }
    }
}