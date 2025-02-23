using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Battle.UnitCharacter.State
{
    public class CharacterUnitAttackState : ActionStateBase
    {
        private int _animIDAttack;
        public override void Init(CharacterUnitView view)
        {
            base.Init(view);
            _animIDAttack = Animator.StringToHash("Attack");
        }

        public override void StateAction()
        {
            view.Animator.SetTrigger(_animIDAttack);
        }
    }
}