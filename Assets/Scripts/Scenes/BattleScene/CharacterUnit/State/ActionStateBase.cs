namespace Scenes.Battle.UnitCharacter.State
{
    public abstract class ActionStateBase
    {
        public CharacterUnitView view;
        public virtual void Init(CharacterUnitView view)
        {
            this.view = view;
        }

        public virtual void StateAction() { }

    }
}