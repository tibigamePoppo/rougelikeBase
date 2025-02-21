using UnityEngine;

namespace Scenes.MainScene.Player
{
    [System.Serializable]
    public struct Card
    {
        public string name;
        public Sprite sprite;
        public string text;
        public float hp;
        public float attack;

        public Card(string name, Sprite sprite, string text,float hp,float attack)
        {
            this.name = name;
            this.sprite = sprite;
            this.text = text;
            this.hp = hp;
            this.attack = attack;
        }
    }
}