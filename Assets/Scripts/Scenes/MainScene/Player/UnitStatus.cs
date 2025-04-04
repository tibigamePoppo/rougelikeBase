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
        public UnitGroup group;
        public float hp;
        public float shield;
        public float attack;
        public float attackRange;
        public float attackSpeed;
        public float speed;

        public UnitStatus(string name, Sprite sprite, string text, UnitWeaponType type, UnitGroup group, float hp, float shield, float attack, float attackRange,float attackSpeed, float speed)
        {
            this.name = name;
            this.sprite = sprite;
            this.text = text;
            this.type = type;
            this.group = group;
            this.hp = hp;
            this.shield = shield;
            this.attack = attack;
            this.attackRange = attackRange;
            this.attackSpeed = attackSpeed;
            this.speed = speed;
        }
    }

    public enum UnitWeaponType
    {
        Range,
        Melee,
        Rider
    }

    public enum UnitGroup
    {
        Player,
        Enemy
    }
}