using UnityEngine;

namespace Scenes.Battle.UnitCharacter.State
{
    public class CharacterUnitDeadState : ActionStateBase
    {
        private int _animIDDead;
        public override void Init(CharacterUnitView view)
        {
            base.Init(view);
            _animIDDead = Animator.StringToHash("Dead");
        }

        public override void StateAction()
        {
            view.Animator.SetTrigger(_animIDDead);
            view.HideHpGauge();
            view.HideGroupColorCircle();
            view.ColliderActive(false);
        }
    }
}