using UnityEngine;

namespace Scenes.MainScene.Player
{
    [System.Serializable]
    public struct UnitStatus
    {
        public string name;
        public Sprite sprite;
        public string text;
        public UnitWeaponType type;
        public float hp;
        public float attack;
        public float attackRange;
        public float speed;

        public UnitStatus(string name, Sprite sprite, string text, UnitWeaponType type, float hp,float attack, float attackRange, float speed)
        {
            this.name = name;
            this.sprite = sprite;
            this.text = text;
            this.type = type;
            this.hp = hp;
            this.attack = attack;
            this.attackRange = attackRange;
            this.speed = speed;
        }
    }

    public enum UnitWeaponType
    {
        Range,
        Melee
    }
}