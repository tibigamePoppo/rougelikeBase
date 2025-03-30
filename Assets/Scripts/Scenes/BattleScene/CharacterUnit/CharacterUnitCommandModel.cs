using UnityEngine;
using UniRx;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System;
using Scenes.Battle.UnitCharacter.State;
using System.Linq;
using System.Threading;
using Scenes.MainScene.Player;

namespace Scenes.Battle.UnitCharacter
{
    public class CharacterUnitCommandModel
    {
        private NavMeshAgent _agent;
        private Transform _originTransfrom;
        private CancellationTokenSource _commandTokenSorce;
        private CharacterUnitModel[] _teamGroup;
        private CharacterUnitModel[] _enemyGroup;
        private MoveCommand _moveCommand;
        private float _attackRange;
        private string _unitName;
        private UnitWeaponType _type;
        private Vector3 _larkPastPosition;

        public void Init(NavMeshAgent agent, Transform originTransfrom, CharacterUnitModel[] teamGroup, CharacterUnitModel[] enemyGroup, IObservable<MoveCommand> commandObservable,float attackRange, string unitName, UnitWeaponType type)
        {
            _originTransfrom = originTransfrom;
            _agent = agent;
            _teamGroup = teamGroup;
            _enemyGroup = enemyGroup;
            commandObservable.Subscribe(c => _moveCommand = c);
            _attackRange = attackRange;
            _unitName = unitName;
            _type = type;
            _larkPastPosition = _originTransfrom.position;
        }

        private void CommandCancel()
        {
            _commandTokenSorce?.Cancel();
            _commandTokenSorce?.Dispose();
            _commandTokenSorce = new CancellationTokenSource();

        }

        public void MoveAtTarget()
        {
            CommandCancel();
            var _targetUnit = GetTarget()[0];
            _agent.SetDestination(_targetUnit.Transform.position);
        }

        public async UniTask MoveAtBack()
        {
            CommandCancel();
            var _targetUnit = GetTarget()[0];
            Vector3 backDirection = (_originTransfrom.position - _targetUnit.Transform.position).normalized;
            var backTeamUnit = _teamGroup.Where(unit => unit.CurrentState != CharacterUnitStateType.Dead && unit.UnitName != _unitName).OrderBy(unit => Vector3.Distance(unit.Transform.position, _originTransfrom.position)).FirstOrDefault(unit => Vector3.Angle((unit.Transform.position - _originTransfrom.position).normalized,backDirection) <= 90);

            Vector3 movePosition = Vector3.zero;
            if(backTeamUnit != null && backTeamUnit != default)
            {
                movePosition = _type == UnitWeaponType.Range ?
                                        backTeamUnit.Transform.position + (_originTransfrom.position + backTeamUnit.Transform.position).normalized :
                                        backTeamUnit.Transform.position + (_originTransfrom.position - backTeamUnit.Transform.position).normalized;
            }
            else
            {
                float backLength = 10f;
                movePosition = _originTransfrom.position + backDirection * backLength;
            }
            _agent.SetDestination(movePosition);

            try
            {
                await UniTask.WaitUntil(() => !IsMoving() || _moveCommand != MoveCommand.Back, cancellationToken: _commandTokenSorce.Token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (_moveCommand == MoveCommand.Back) _moveCommand = MoveCommand.Stop;
            }
        }

        public async UniTask MoveAtLark()
        {
            CommandCancel();
            Vector3 movePosition = Vector3.zero;
            var taregt = GetTarget();
            var targetUnit = GetTarget()[0];
            try
            {
                if (_type == UnitWeaponType.Rider)
                {
                    Vector3 enemyCenter = taregt.Select(t => t.Transform.position).Aggregate(Vector3.zero, (sum, point) => sum + point) / taregt.Length;
                    Vector3 farestEnemy = taregt.Select(t => t.Transform.position).Last();
                    Vector3 larkDirection = (enemyCenter - farestEnemy).normalized;
                    larkDirection = new Vector3(-larkDirection.x, 0, Mathf.Abs(larkDirection.x)).normalized;
                    //float enemyMaxRange = Mathf.Min(taregt.Max(t => t.AttackRange),10);
                    float enemyMaxRange = Mathf.Max(Vector3.Distance(farestEnemy,_originTransfrom.position), 10);
                    float larkLength = enemyMaxRange + 5;
                    movePosition = enemyCenter + larkDirection * larkLength;
                    Vector3 perpendicularDirection = new Vector3(-larkDirection.z, 0, larkDirection.x);
                    Vector3 checkPoint = enemyCenter + perpendicularDirection.normalized * larkLength;

                    _agent.SetDestination(checkPoint);
                    await UniTask.WaitUntil(() => !IsMoving(1) || taregt[0].CurrentState == CharacterUnitStateType.Attak, cancellationToken: _commandTokenSorce.Token);
                    _agent.SetDestination(movePosition);
                    taregt = GetTarget();
                    await UniTask.WaitUntil(() => taregt[0].CurrentState == CharacterUnitStateType.Attak, cancellationToken: _commandTokenSorce.Token);
                    _agent.SetDestination(taregt[0].Transform.position);
                }
                else if (_type == UnitWeaponType.Melee)
                {
                    Vector3 backDirection = (_originTransfrom.position - targetUnit.Transform.position).normalized;
                    var backTeamUnit = _teamGroup.Where(unit => unit.CurrentState != CharacterUnitStateType.Dead && unit.UnitName != _unitName && unit.WeaponType == UnitWeaponType.Range).OrderBy(unit => Vector3.Distance(unit.Transform.position, _originTransfrom.position)).FirstOrDefault(unit => Vector3.Angle((unit.Transform.position - _originTransfrom.position).normalized, backDirection) <= 90);
                    if (backTeamUnit != null && backTeamUnit != default)
                    {
                        Vector3 offset = (_originTransfrom.position - backTeamUnit.Transform.position).normalized;
                        offset = new Vector3(offset.x, offset.y, 2);
                        movePosition = backTeamUnit.Transform.position + offset;
                        _agent.SetDestination(movePosition);
                    }
                    else
                    {
                        _agent.SetDestination(movePosition);
                        await UniTask.WaitUntil(() => taregt[0].CurrentState == CharacterUnitStateType.Attak, cancellationToken: _commandTokenSorce.Token);
                        _agent.SetDestination(_larkPastPosition);
                        await UniTask.WaitUntil(() => !IsMoving(1), cancellationToken: _commandTokenSorce.Token);
                    }
                }
                else if (_type == UnitWeaponType.Range)
                {
                    float targetDistance = Vector3.Distance(targetUnit.Transform.position, _originTransfrom.position);

                    if (targetDistance > _attackRange)
                    {
                        _agent.SetDestination(targetUnit.Transform.position);
                        await UniTask.WaitUntil(() => Vector3.Distance(targetUnit.Transform.position, _originTransfrom.position) <= _attackRange);
                    }
                    else if (Vector3.Distance(_larkPastPosition, _originTransfrom.position) > 1)
                    {
                        _agent.SetDestination(_larkPastPosition);
                        await UniTask.WaitUntil(() => Vector3.Distance(targetUnit.Transform.position, _originTransfrom.position) > _attackRange || !IsMoving(1));
                    }
                    else
                    {
                        Vector3 backDirection = (_originTransfrom.position - targetUnit.Transform.position).normalized;
                        float backLength = 10f;
                        movePosition = _originTransfrom.position + backDirection * backLength;
                        _agent.SetDestination(movePosition);
                        await UniTask.WaitUntil(() => Vector3.Distance(targetUnit.Transform.position, _originTransfrom.position) > _attackRange || !IsMoving(1));
                    }
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                _larkPastPosition = _originTransfrom.position;
            }
        }

        public CharacterUnitModel[] GetTarget()
        {
            return _enemyGroup.Where(enemy => enemy.CurrentState != CharacterUnitStateType.Dead).OrderBy(enemy => Vector3.Distance(enemy.Transform.position, _originTransfrom.position)).ToArray();
        }

        private float TargetDistance(CharacterUnitModel model)
        {
            return Vector3.Distance(_originTransfrom.position, model.Transform.position);
        }

        private bool IsMoving(float distance = 0.2f)
        {
            if (!_agent.isActiveAndEnabled) return false;
            return _agent.remainingDistance > distance;
        }
    }
}