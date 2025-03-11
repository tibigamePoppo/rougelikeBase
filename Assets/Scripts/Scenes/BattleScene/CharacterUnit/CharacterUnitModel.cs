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
        private float _maxShield;
        private string _unitName;
        private UnitWeaponType _type;
        private UnitGroup _unitGroup;

        private const float REACTIONDISTANCE = 3f;
        private Transform _originTransfrom;
        private bool isHoldPosition = true;
        private bool isHoldFormationPosition = false;
        private Vector3 _holdPosition;
        private CancellationTokenSource formationMoveSorce ;

        private ReactiveProperty<float> _health = new ReactiveProperty<float>();
        private ReactiveProperty<float> _shield = new ReactiveProperty<float>();
        private Subject<float> _getDamage = new Subject<float>();
        private Subject<float> _getHeal = new Subject<float>();
        private ReactiveProperty<CharacterUnitStateType> _stateType = new ReactiveProperty<CharacterUnitStateType>(CharacterUnitStateType.Idle);
        private Subject<AttackArg> _attackTarget = new Subject<AttackArg>();
        private NavMeshAgent _agent;
        private CharacterUnitModel[] _teamGroup;
        private CharacterUnitModel[] _enemyGroup;
        private CharacterUnitModel _targetUnit;
        private const float ATTACKSPEED = 2f;

        public UnitWeaponType WeaponType { get { return _type; } }
        public IObservable<float> OnChangeHealth => _health;
        public IObservable<float> OnChangeSheild => _shield;
        public IObservable<float> OnDamage => _getDamage;
        public IObservable<float> OnHeal => _getHeal;
        public IObservable<CharacterUnitStateType> OnChangeStateType => _stateType;

        public IObservable<AttackArg> OnAttackTarget => _attackTarget;
        public string UnitName { get { return _unitName; } }
        public float AttackPower { get { return _attackPower; } }
        public float CurrentHealth { get { return _health.Value; } }
        public float MaxHealth { get { return _maxHp; } }
        public float MaxSheild { get { return _maxShield; } }
        public Transform Transform { get { return _originTransfrom; } }
        public CharacterUnitModel TargetUnit { get { return _targetUnit; } }
        public CharacterUnitStateType CurrentState { get { return _stateType.Value; } }

        public Vector3 TargetPosition { get { return Transform.position; } }

        public void Init(UnitStatus status, NavMeshAgent agent, Transform originTransfrom)
        {
            formationMoveSorce = new CancellationTokenSource();
            _unitName = status.name;
            _maxHp = status.hp;
            _maxShield = status.shield;
            _health.Value = status.hp;
            _shield.Value = status.shield;
            _attackPower = status.attack;
            _attackRange = status.attackRange;
            _type = status.type;
            _unitGroup = status.group;
            _originTransfrom = originTransfrom;
            _agent = agent;
            _agent.speed = status.speed;


            if (HasRelicItem(4))
            {
                _maxShield += _maxHp * 0.2f;
                _shield.Value += _maxHp * 0.2f;
            }
        }

        public void BattleLoopStart()
        {
            _holdPosition = Transform.position;
            MainLoop().Forget();
            if(HasRelicItem(7))
            {
                SubAuotHealLoop().Forget();
            }
            if(HasRelicItem(5)) HealPosition().Forget();
        }

        public void Charge()
        {
            isHoldPosition = false;
        }


        public void SetGroup(CharacterUnitModel[] teamGroup, CharacterUnitModel[] enemyGroup)
        {
            _teamGroup = teamGroup;
            _enemyGroup = enemyGroup;
        }

        public async UniTaskVoid Attack(IDamagable target,int effect = 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(AttackSpeed() * 0.2f));

            target.TakeDamage(_attackPower);
            if (HasRelicItem(6))
            {
                GetHeal(_attackPower * 0.1f);
            }
            _attackTarget.OnNext(new AttackArg(target.TargetPosition, effect));

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
                    if (HasRelicItem(6))
                    {
                        GetHeal(_attackPower * 0.1f);
                    }
                    _attackTarget.OnNext(new AttackArg(target.TargetPosition, 1));
                }
            }
        }

        public void TakeDamage(float damage)
        {
            if (_shield.Value > 0)
            {
                _shield.Value = _shield.Value - damage <= 0 ? 0 : _shield.Value - damage;
            }
            else
            {
                _health.Value = _health.Value - damage <= 0 ? 0 : _health.Value - damage;
            }
            _getDamage.OnNext(damage);
            if (_health.Value <= 0)
            {
                _agent.isStopped = true;
                ChangeState(CharacterUnitStateType.Dead);
            }
        }

        public void GetHeal(float value)
        {
            if (_stateType.Value == CharacterUnitStateType.Dead) return;
            var healValue = _health.Value + value >= MaxHealth ? (_health.Value + value >= MaxHealth ? MaxHealth : _health.Value + value) - MaxHealth : value;
            if (healValue > 0)
            {
                _getHeal.OnNext(healValue);
                _health.Value += healValue;
            }
        }

        private async UniTaskVoid SubAuotHealLoop()
        {
            while (_health.Value > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                GetHeal(MaxHealth * 0.05f);
            }
        }
        private async UniTaskVoid HealPosition()
        {
            await UniTask.WaitUntil(() => _health.Value / MaxHealth <= 0.5f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            GetHeal(MaxHealth * 0.2f);
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
                    Attack(taregt[0]).Forget();
                    if(_type == UnitWeaponType.Range)
                    {
                        if(HasRelicItem(2) && attackableTarget.Length > 1)
                        {
                            Attack(taregt[1],2).Forget();
                        }
                    }
                    await UniTask.Delay(TimeSpan.FromSeconds(AttackSpeed()));
                    ChangeState(CharacterUnitStateType.Idle);
                }
                else if (isHoldPosition && !ReactionInEnemy() && IsHoldPosition()) // Idle
                {
                    if (_holdPosition != Transform.position) _holdPosition = Transform.position;
                    _agent.isStopped = true;
                    ChangeState(CharacterUnitStateType.Idle);
                }
                else // Move
                {
                    _agent.isStopped = false;
                    ChangeState(CharacterUnitStateType.Move);
                    if (!isHoldFormationPosition)
                    {
                        if (ReactionInEnemy() || !isHoldPosition) MoveAtTarget();
                        else MoveAtHoldPosition();
                    }
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

        private void MoveAtHoldPosition()
        {
            _agent.SetDestination(_holdPosition); 
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
            return PlayerSingleton.Instance.CurrentRelic.Select(c => c.relicItemId).Contains(id);
        }

        private bool ReactionInEnemy()
        {
            return Vector3.Distance(_targetUnit.Transform.position, Transform.position) <= REACTIONDISTANCE;
        }

        private bool IsHoldPosition()
        {
            return Vector3.Distance(_holdPosition, Transform.position) <= 0.2f;
        }
    }

    public class AttackArg
    {
        public Vector3 target;
        public int effectNum;
        public AttackArg(Vector3 target, int effectNum)
        {
            this.target = target;
            this.effectNum = effectNum;
        }

    }
}