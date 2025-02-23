using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Battle.UnitCharacter.State
{
    public enum CharacterUnitStateType
    {
        Idle,
        Move,
        Attak,
        Dead
    }


    public class CharacterUnitStateController
    {
        public void Init(CharacterUnitModel model, CharacterUnitView view)
        {
            CharacterUnitIdleState idleState = new CharacterUnitIdleState();
            CharacterUnitAttackState attackState = new CharacterUnitAttackState();
            CharacterUnitMoveState moveState = new CharacterUnitMoveState();
            CharacterUnitDeadState deadState = new CharacterUnitDeadState ();

            idleState.Init(view);
            attackState.Init(view);
            moveState.Init(view);
            deadState.Init(view);

            model.OnChangeStateType.Subscribe(state =>
            {
                switch (state)
                {
                    case CharacterUnitStateType.Idle:
                        idleState.StateAction();
                        break;
                    case CharacterUnitStateType.Move:
                        moveState.StateAction();
                        break;
                    case CharacterUnitStateType.Attak:
                        attackState.StateAction();
                        break;
                    case CharacterUnitStateType.Dead:
                        deadState.StateAction();
                        break;
                    default:
                        break;
                }
            }).AddTo(view.gameObject);
        }
    }
}