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
using System.Threading;

namespace Scenes.Battle.UnitCharacter
{
    public class CharacterUnitModel : IDamagable, IAttacker
    {
        private float _maxHp;
        private float _attackPower;
        private float _attackRange;
        private string _unitName;
        private UnitWeaponType _type;
        private UnitGroup _unitGroup;

        private Transform _originTransfrom;
        private bool isHoldPosition = false;
        private bool isHoldFormationPosition = false;
        private CancellationTokenSource formationMoveSorce ;

        private ReactiveProperty<float> _health = new ReactiveProperty<float>();
        private ReactiveProperty<CharacterUnitStateType> _stateType = new ReactiveProperty<CharacterUnitStateType>(CharacterUnitStateType.Idle);
        private Subject<Vector3> _attackTarget = new Subject<Vector3>();
        private NavMeshAgent _agent;
        private CharacterUnitModel[] _teamGroup;
        private CharacterUnitModel[] _enemyGroup;
        private CharacterUnitModel _targetUnit;
        private const float ATTACKSPEED = 2f;

        public UnitWeaponType WeaponType { get { return _type; } }
        public IObservable<float> OnChangeHealth => _health;
        public IObservable<CharacterUnitStateType> OnChangeStateType => _stateType;

        public IObservable<Vector3> OnAttackTarget => _attackTarget;
        public string UnitName { get { return _unitName; } }
        public float AttackPower { get { return _attackPower; } }
        public float CurrentHealth { get { return _health.Value; } }
        public float MaxHealth { get { return _maxHp; } }
        public Transform Transform { get { return _originTransfrom; } }
        public CharacterUnitModel TargetUnit { get { return _targetUnit; } }
        public CharacterUnitStateType CurrentState { get { return _stateType.Value; } }

        public Vector3 TargetPosition { get { return Transform.position; } }

        public void Init(UnitStatus status, NavMeshAgent agent, Transform originTransfrom)
        {
            formationMoveSorce = new CancellationTokenSource();
            _unitName = status.name;
            _maxHp = status.hp;
            _health.Value = status.hp;
            _attackPower = status.attack;
            _attackRange = status.attackRange;
            _type = status.type;
            _unitGroup = status.group;
            _originTransfrom = originTransfrom;
            _agent = agent;
            _agent.speed = status.speed;
        }

        public void BattleLoopStart()
        {
            MainLoop().Forget();
        }


        public void SetGroup(CharacterUnitModel[] teamGroup, CharacterUnitModel[] enemyGroup)
        {
            _teamGroup = teamGroup;
            _enemyGroup = enemyGroup;
        }

        public async UniTaskVoid Attack(IDamagable target)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(AttackSpeed() * 0.2f));

            target.TakeDamage(_attackPower);
            _attackTarget.OnNext(target.TargetPosition);

            if (GetTarget().Length <= 0) return;
            if (_type == UnitWeaponType.Range)
            {
                if (HasRelicItem(1))
                {
                    ChangeState(CharacterUnitStateType.Idle);
                    await UniTask.Delay(TimeSpan.FromTicks(1));
                    ChangeState(CharacterUnitStateType.Attak);
                    await UniTask.Delay(TimeSpan.FromSeconds(AttackSpeed() * 0.2f));
                    target.TakeDamage(_attackPower);
                    _attackTarget.OnNext(target.TargetPosition);
                }
            }
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
                if (taregt.Length <= 0)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
                    continue;
                }
                else if (_targetUnit != taregt[0])
                {
                    _targetUnit = taregt[0];
                }
                var attackableTarget = taregt.Where(enemy => Vector3.Distance(enemy.Transform.position, Transform.position) <= _attackRange).ToArray();
                if (attackableTarget.Length > 0) // Attack
                {
                    ChangeState(CharacterUnitStateType.Attak);
                    _agent.isStopped = true;
                    Attack(taregt[0]);
                    if(_type == UnitWeaponType.Range)
                    {
                        if(HasRelicItem(2) && attackableTarget.Length > 1)
                        {
                            Attack(taregt[1]);
                        }
                    }
                    await UniTask.Delay(TimeSpan.FromSeconds(AttackSpeed()));
                    ChangeState(CharacterUnitStateType.Idle);
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
                    if (!isHoldFormationPosition) MoveAtTarget();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }

        public CharacterUnitModel[] GetTarget()
        {
            return _enemyGroup.Where(enemy => enemy.CurrentState != CharacterUnitStateType.Dead).OrderBy(enemy => Vector3.Distance(enemy.Transform.position, Transform.position)).ToArray();
        }

        private void ChangeState(CharacterUnitStateType state)
        {
            if (_stateType.Value != state && _stateType.Value != CharacterUnitStateType.Dead) _stateType.Value = state;
        }

        private void MoveAtTarget()
        {
            _agent.SetDestination(_targetUnit.Transform.position);
        }

        public async UniTaskVoid SetFormationPoint(Vector3 vector)
        {
            formationMoveSorce.Cancel();

            _agent.SetDestination(vector);
            isHoldFormationPosition = true;
            await UniTask.WaitUntil(() => Vector3.Distance(Transform.position, vector) < 1, cancellationToken: formationMoveSorce.Token);
            isHoldFormationPosition = false;
        }

        private float AttackSpeed()
        {
            return (ATTACKSPEED *
                (HasRelicItem(3) && _type == UnitWeaponType.Range ? 0.3f : 1f));
        }

        private bool HasRelicItem(int id)
        {
            if (_unitGroup == UnitGroup.Enemy) return false;
            return PlayerSingleton.Instance.CurrentRelic.Select(c => c._relicItemId).Contains(id);
        }

    }
}