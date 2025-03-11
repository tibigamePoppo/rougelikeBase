using UnityEngine;

namespace Scenes.Battle.UnitCharacter
{
    public interface IDamagable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; } 
        void TakeDamage(float damage,Color color);
        void GetHeal(float value);
        Vector3 TargetPosition { get; }
    }
}