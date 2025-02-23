using UnityEngine;

namespace Scenes.MainScene.Player
{
    [System.Serializable]
    public struct  UnitStatus
    {
        public string name;
        public Sprite sprite;
        public string text;
        public float hp;
        public float attack;
        public float attackRange;

        public UnitStatus(string name, Sprite sprite, string text,float hp,float attack, float attackRange)
        {
            this.name = name;
            this.sprite = sprite;
            this.text = text;
            this.hp = hp;
            this.attack = attack;
            this.attackRange = attackRange;
        }
    }
}