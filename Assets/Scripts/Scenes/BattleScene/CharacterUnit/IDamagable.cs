using UnityEngine;

namespace Scenes.Battle.UnitCharacter
{
    public interface IDamagable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; } 
        void TakeDamage(float damage);
        void GetHeal(float value);
        Vector3 TargetPosition { get; }
    }
}