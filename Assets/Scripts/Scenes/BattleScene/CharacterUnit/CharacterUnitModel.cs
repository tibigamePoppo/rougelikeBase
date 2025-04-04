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
        private float _attackSpeed;
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
        private Subject<bool> _isSelect = new Subject<bool>();
        private ReactiveProperty<CharacterUnitStateType> _stateType = new ReactiveProperty<CharacterUnitStateType>(CharacterUnitStateType.Idle);
        private Subject<AttackArg> _attackTarget = new Subject<AttackArg>();
        private NavMeshAgent _agent;
        private CharacterUnitModel[] _teamGroup;
        private CharacterUnitModel[] _enemyGroup;
        private CharacterUnitModel _targetUnit;
        private Color orangeColor = new Color(1, 0.8706f, 0.2392f);
        private Color pinkColor = new Color(0.8745f,0.3216f,0.5255f);
        private Color blueColor = new Color(0.2392f, 0.4078f, 1f);
        private Color redColor = new Color(1f, 0.3f, 0.32f); 

        private CharacterUnitCommandModel _commandModel;

        public UnitWeaponType WeaponType { get { return _type; } }
        public IObservable<MoveCommand> OnChangeCommand => _moveCommand;
        public IObservable<float> OnChangeHealth => _health;
        public IObservable<float> OnChangeSheild => _shield;
        public IObservable<DamageArg> OnDamage => _getDamage;
        public IObservable<float> OnHeal => _getHeal;
        public IObservable<bool> OnIsSelect => _isSelect;
        public IObservable<CharacterUnitStateType> OnChangeStateType => _stateType;

        public IObservable<AttackArg> OnAttackTarget => _attackTarget;
        public string UnitName { get { return _unitName; } }
        public float AttackPower { get { return _attackPower; } }
        public float CurrentHealth { get { return _health.Value; } }
        public float MaxHealth { get { return _maxHp; } }
        public float MaxSheild { get { return _maxShield; } }
        public float AttackRange { get { return _attackRange; } }
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
            _attackSpeed = status.attackSpeed;
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
            _moveCommand.Value = MoveCommand.Charge;
        }

        public void Back()
        {
            _moveCommand.Value = MoveCommand.Back;
        }

        public void Lark()
        {
            _moveCommand.Value = MoveCommand.Lark;
        }

        public void SetGroup(CharacterUnitModel[] teamGroup, CharacterUnitModel[] enemyGroup)
        {
            _teamGroup = teamGroup;
            _enemyGroup = enemyGroup;

            _commandModel = new CharacterUnitCommandModel();
            _commandModel.Init(_agent, _originTransfrom, _teamGroup, _enemyGroup, OnChangeCommand, _attackRange, _unitName, _type);
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
            if (_unitGroup == UnitGroup.Player) color = redColor;
            var damageAnimation = _unitGroup == UnitGroup.Player ? DamageAnimation.Shake : DamageAnimation.Jump;
            _getDamage.OnNext(new DamageArg(damage, color, damageAnimation));
            if (_health.Value <= 0)
            {
                ChangeState(CharacterUnitStateType.Dead);
                if(_agent.enabled)
                {
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
                if (attackableTarget.Length > 0 &&
                    _moveCommand.Value != MoveCommand.Back &&
                    !(_moveCommand.Value == MoveCommand.Lark && Vector3.Distance(attackableTarget[0].Transform.position,Transform.position) < (_attackRange - 1) && _type == UnitWeaponType.Range)) // Attack
                {
                    await MainLoopAttackAction(attackableTarget);
                }
                else if (_moveCommand.Value == MoveCommand.Stop && !ReactionInEnemy() && !IsMoving()) // Idle
                {
                    _agent.isStopped = true;
                    ChangeState(CharacterUnitStateType.Idle);
                }
                else // Move
                {
                    _agent.isStopped = false;
                    ChangeState(CharacterUnitStateType.Move);
                    if (!isHoldFormationPosition)
                    {
                        if (_moveCommand.Value == MoveCommand.Back) await _commandModel.MoveAtBack();
                        else if (ReactionInEnemy() || _moveCommand.Value == MoveCommand.Charge) _commandModel.MoveAtTarget();
                        else if (_moveCommand.Value == MoveCommand.Lark) await _commandModel.MoveAtLark();
                        else if (!IsMoving()) _moveCommand.Value = MoveCommand.Stop;
                        else MoveAtHoldPosition();
                        
                    }
                }

                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }

        public async UniTask MainLoopAttackAction(CharacterUnitModel[] attackableTarget)
        {
            var taregt = GetTarget();
            ChangeState(CharacterUnitStateType.Attak);
            _agent.isStopped = true;
            Attack(taregt[0], orangeColor).Forget();
            if (_type == UnitWeaponType.Range)
            {
                if (HasRelicItem(2) && attackableTarget.Length > 1)
                {
                    Attack(taregt[1], blueColor, 2).Forget();
                }
            }
            await UniTask.Delay(TimeSpan.FromSeconds(AttackSpeed()));
            ChangeState(CharacterUnitStateType.Idle);
        }


        public CharacterUnitModel[] GetTarget()
        {
            return _enemyGroup.Where(enemy => enemy.CurrentState != CharacterUnitStateType.Dead).OrderBy(enemy => Vector3.Distance(enemy.Transform.position, Transform.position)).ToArray();
        }

        private void ChangeState(CharacterUnitStateType state)
        {
            if (_stateType.Value != state && _stateType.Value != CharacterUnitStateType.Dead) _stateType.Value = state;
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
            return (_attackSpeed *
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

        public void IsSelect(bool value)
        {
            _isSelect.OnNext(value);
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
        public DamageAnimation damageAnimation;
        public DamageArg(float damage,Color color, DamageAnimation damageAnimation)
        {
            this.damage = damage;
            this.color = color;
            this.damageAnimation = damageAnimation;
        }
    }

    public enum DamageAnimation
    {
        Jump,
        Shake
    }
}