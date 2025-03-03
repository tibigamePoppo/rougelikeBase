using Cysharp.Threading.Tasks;

namespace Scenes.Battle.UnitCharacter
{
    public interface  IAttacker
    {
        float AttackPower { get; }
        UniTaskVoid Attack(IDamagable target,int effect = 0);
    }
}