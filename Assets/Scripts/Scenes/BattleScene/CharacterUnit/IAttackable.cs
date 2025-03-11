using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scenes.Battle.UnitCharacter
{
    public interface  IAttacker
    {
        float AttackPower { get; }
        UniTaskVoid Attack(IDamagable target,Color color,int effect = 0);
    }
}