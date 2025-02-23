namespace Scenes.Battle.UnitCharacter
{
    public interface IAttacker
    {
        float AttackPower { get; }
        void Attack(IDamagable target);
    }
}