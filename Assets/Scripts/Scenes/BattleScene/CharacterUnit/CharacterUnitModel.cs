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
        private bool isHoldFormationPosition = false;
        private Vector3 _holdPosition;
        private CancellationTokenSource formationMoveSorce;
        private ReactiveProperty<MoveCommand> _moveCommand = new ReactiveProperty<MoveCommand>(MoveCommand.Stop);

        private ReactiveProperty<float> _health = new ReactiveProperty<float>();
        private ReactiveProperty<float> _shield = new ReactiveProperty<float>();
        private Subject<DamageArg> _getDamage = new Subject<DamageArg>();
        private Subject<float> _getHeal = new Subject<float>();
        private ReactiveProperty<CharacterUnitStateType> _stateType = new ReactiveProperty<CharacterUnitStateType>(CharacterUnitStateType.Idle);
        private Subject<AttackArg> _attackTarget = new Subject<AttackArg>();
        private NavMeshAgent _agent;
        private CharacterUnitModel[] _teamGroup;
        private CharacterUnitModel[] _enemyGroup;
        private CharacterUnitModel _targetUnit;
        private const float ATTACKSPEED = 2f;
        private Color orangeColor = new Color(1, 0.8706f, 0.2392f);
        private Color pinkColor = new Color(0.8745f,0.3216f,0.5255f);
        private Color blueColor = new Color(0.2392f, 0.4078f, 1f);

        private CancellationTokenSource _commandTokenSorce;

        public UnitWeaponType WeaponType { get { return _type; } }
        public IObservable<MoveCommand> OnChangeCommand => _moveCommand;
        public IObservable<float> OnChangeHealth => _health;
        public IObservable<float> OnChangeSheild => _shield;
        public IObservable<DamageArg> OnDamage => _getDamage;
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
            CommandCancel();
            _moveCommand.Value = MoveCommand.Charge;
        }

        public void Back()
        {
            CommandCancel();
            _moveCommand.Value = MoveCommand.Back;
        }

        public void Lark()
        {
            CommandCancel();
            _moveCommand.Value = MoveCommand.Lark;
        }

        private void CommandCancel()
        {
            _commandTokenSorce?.Cancel();
            _commandTokenSorce?.Dispose();
            _commandTokenSorce = new CancellationTokenSource();

        }

        public void SetGroup(CharacterUnitModel[] teamGroup, CharacterUnitModel[] enemyGroup)
        {
            _teamGroup = teamGroup;
            _enemyGroup = enemyGroup;
        }

        public async UniTaskVoid Attack(IDamagable target, Color color, int effect = 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(AttackSpeed() * 0.2f));

            target.TakeDamage(_attackPower, color);
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
                    target.TakeDamage(_attackPower, pinkColor);
                    if (HasRelicItem(6))
                    {
                        GetHeal(_attackPower * 0.1f);
                    }
                    _attackTarget.OnNext(new AttackArg(target.TargetPosition, 1));
                }
            }
        }

        public void TakeDamage(float damage,Color color)
        {
            if (_shield.Value > 0)
            {
                _shield.Value = _shield.Value - damage <= 0 ? 0 : _shield.Value - damage;
            }
            else
            {
                _health.Value = _health.Value - damage <= 0 ? 0 : _health.Value - damage;
            }
            _getDamage.OnNext(new DamageArg(damage, color));
            if (_health.Value <= 0)
            {
                ChangeState(CharacterUnitStateType.Dead);
                if(_agent.enabled)
                {
                    _commandTokenSorce.Cancel();
                    _commandTokenSorce.Dispose();
                    _agent.isStopped = true;
                    _agent.enabled = false;
                }
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
                if (attackableTarget.Length > 0 && _moveCommand.Value != MoveCommand.Back) // Attack
                {
                    ChangeState(CharacterUnitStateType.Attak);
                    _agent.isStopped = true;
                    Attack(taregt[0], orangeColor).Forget();
                    if(_type == UnitWeaponType.Range)
                    {
                        if(HasRelicItem(2) && attackableTarget.Length > 1)
                        {
                            Attack(taregt[1], blueColor, 2).Forget();
                        }
                    }
                    await UniTask.Delay(TimeSpan.FromSeconds(AttackSpeed()));
                    ChangeState(CharacterUnitStateType.Idle);
                }
                else if (_moveCommand.Value == MoveCommand.Stop && !ReactionInEnemy() && !IsMoving()) // Idle
                {
                    //if (_holdPosition != Transform.position) _holdPosition = Transform.position;
                    _agent.isStopped = true;
                    ChangeState(CharacterUnitStateType.Idle);
                }
                else // Move
                {
                    _agent.isStopped = false;
                    ChangeState(CharacterUnitStateType.Move);
                    if (!isHoldFormationPosition)
                    {
                        if (_moveCommand.Value == MoveCommand.Back) await MoveAtBack();
                        else if (ReactionInEnemy() || _moveCommand.Value == MoveCommand.Charge) MoveAtTarget();
                        else if (_moveCommand.Value == MoveCommand.Lark) await MoveAtLark();
                        else if (!IsMoving()) _moveCommand.Value = MoveCommand.Stop;
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

        private async UniTask MoveAtBack()
        {
            Vector3 backDirection = (Transform.position - _targetUnit.Transform.position).normalized;
            float backLength = 10f;
            _agent.SetDestination(Transform.position + backDirection * backLength);

            try
            {
                await UniTask.WaitUntil(() => !IsMoving() || _moveCommand.Value != MoveCommand.Back, cancellationToken: _commandTokenSorce.Token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (_moveCommand.Value == MoveCommand.Back) _moveCommand.Value = MoveCommand.Stop;
            }
        }

        private async UniTask MoveAtLark()
        {
            var taregt = GetTarget();
            Vector3 center = taregt.Select(t => t.Transform.position).Aggregate(Vector3.zero, (sum, point) => sum + point) / taregt.Length;
            Vector3 larkDirection = (center - Transform.position).normalized;
            larkDirection = new Vector3(-larkDirection.x, 0, Mathf.Abs(larkDirection.x)).normalized;

            float backLength = 10f;
            _agent.SetDestination(Transform.position + larkDirection * backLength);
            float currentDistance = TargetDistance(taregt[taregt.Length - 1]);
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: _commandTokenSorce.Token);

                while (true)
                {
                    if (_commandTokenSorce.Token.IsCancellationRequested || !IsMoving(_attackRange) || _moveCommand.Value != MoveCommand.Lark) break;
                    center = taregt.Select(t => t.Transform.position).Aggregate(Vector3.zero, (sum, point) => sum + point) / taregt.Length;
                    var farthestPoint = taregt.Select(t => t.Transform.position).OrderByDescending(point => Vector3.Distance(center, point)).First();
                    _agent.SetDestination(farthestPoint);

                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (_moveCommand.Value == MoveCommand.Lark) _moveCommand.Value = MoveCommand.Charge;
            }
        }

        private float TargetDistance(CharacterUnitModel model)
        {
            return Vector3.Distance(Transform.position, model.Transform.position);
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

        private bool IsMoving(float distance = 0.2f)
        {
            return _agent.remainingDistance > distance;
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

    public class DamageArg
    {
        public float damage;
        public Color color;
        public DamageArg(float damage,Color color)
        {
            this.damage = damage;
            this.color = color;
        }
    }
}