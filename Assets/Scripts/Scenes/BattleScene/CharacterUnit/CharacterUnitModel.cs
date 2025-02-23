using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Scenes.MainScene.Player;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System;
using Scenes.Battle.UnitCharacter.State;
using System.Linq;

namespace Scenes.Battle.UnitCharacter
{
    public class CharacterUnitModel : IDamagable, IAttacker
    {
        private float _maxHp;
        private float _attackPower;
        private float _attackRange;

        private Transform _originTransfrom;
        private bool isHoldPosition = false;

        private ReactiveProperty<float> _health = new ReactiveProperty<float>();
        private ReactiveProperty<CharacterUnitStateType> _stateType = new ReactiveProperty<CharacterUnitStateType>();
        private NavMeshAgent _agent;
        private CharacterUnitModel[] _teamGroup;
        private CharacterUnitModel[] _enemyGroup;
        private CharacterUnitModel _targetUnit;

        public IObservable<float> OnChangeHealth => _health;
        public IObservable<CharacterUnitStateType> OnChangeStateType => _stateType;

        public float AttackPower { get { return _attackPower; } }
        public float CurrentHealth { get { return _health.Value; } }
        public float MaxHealth { get { return _maxHp; } }
        public Transform Transform { get { return _originTransfrom; } }
        public CharacterUnitModel TargetUnit { get { return _targetUnit; } }
        public CharacterUnitStateType CurrentState { get { return _stateType.Value; } }

        public void Init(UnitStatus status, NavMeshAgent agent, Transform originTransfrom)
        {
            _maxHp = status.hp;
            _health.Value = status.hp;
            _attackPower = status.attack;
            _attackRange = status.attackRange;
            _originTransfrom = originTransfrom;
            _agent = agent;
            _stateType.Value = CharacterUnitStateType.Idle;
            MainLoop().Forget();
        }

        public void SetGroup(CharacterUnitModel[] teamGroup, CharacterUnitModel[] enemyGroup)
        {
            _teamGroup = teamGroup;
            _enemyGroup = enemyGroup;
        }

        public void Attack(IDamagable target)
        {
            target.TakeDamage(_attackPower);
        }

        public void TakeDamage(float damage)
        {
            _health.Value = _health.Value - damage <= 0 ? 0 : _health.Value - damage;
            if(_health.Value <= 0)
            {
                ChangeState(CharacterUnitStateType.Dead);
            }

        }

        private async UniTaskVoid MainLoop()
        {
            await UniTask.WaitUntil(() => _enemyGroup != null && _teamGroup != null);
            while (_health.Value > 0)
            {
                var taregt = GetTarget();
                if (taregt == null || taregt == default)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
                    continue;
                }
                else if(_targetUnit != taregt)
                {
                    _targetUnit = taregt;
                }

                if (Vector3.Distance(_originTransfrom.position, taregt.Transform.position) <= _attackRange) // Attack
                {
                    ChangeState(CharacterUnitStateType.Attak);
                    Attack(taregt);
                    _agent.isStopped = true;
                    await UniTask.Delay(TimeSpan.FromSeconds(1f));
                }
                else if (isHoldPosition) // Idle
                {
                    _agent.isStopped = true;
                    ChangeState(CharacterUnitStateType.Idle);
                }
                else // Move
                {
                    _agent.isStopped = false;
                    ChangeState(CharacterUnitStateType.Move);
                    MoveAtTarget();
                }
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }

        public CharacterUnitModel GetTarget()
        {
            CharacterUnitModel target = _enemyGroup.Where(enemy => enemy.CurrentState != CharacterUnitStateType.Dead).OrderBy(enemy => Vector3.Distance(enemy.Transform.position, Transform.position)).FirstOrDefault();
            return target;
        }

        private void ChangeState(CharacterUnitStateType state)
        {
            if (_stateType.Value != state && _stateType.Value != CharacterUnitStateType.Dead) _stateType.Value = state;
        }

        private void MoveAtTarget()
        {
            _agent.SetDestination(_targetUnit.Transform.position);
        }

    }
}