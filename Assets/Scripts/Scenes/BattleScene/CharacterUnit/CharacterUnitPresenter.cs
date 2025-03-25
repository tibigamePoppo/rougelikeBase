using System.Collections;
using System.Collections.Generic;
using Scenes.MainScene.Player;
using UnityEngine;
using UniRx;
using UnityEngine.AI;
using Scenes.Battle.UnitCharacter.State;

namespace Scenes.Battle.UnitCharacter
{
    public class CharacterUnitPresenter : MonoBehaviour
    {
        private CharacterUnitModel _model;
        private CharacterUnitView _view;
        private CharacterUnitStateController _stateController;
        private NavMeshAgent _agent;
        public CharacterUnitModel CharacterUnitModel {  get { return _model; } }

        public CharacterUnitPresenter Init(UnitStatus status)
        {
            _model = new CharacterUnitModel();
            _stateController = new CharacterUnitStateController();
            _view = GetComponent<CharacterUnitView>();
            _agent = GetComponent<NavMeshAgent>();

            _model.Init(status,_agent, this.transform);
            _view.Init(_agent);
            _view.OnMoveFormationPoint.Subscribe(p => _model.SetFormationPoint(p)).AddTo(this);

            _stateController.Init(_model,_view);

            _model.OnChangeSheild.Subscribe(s => _view.UpdateSheildGauge(s / _model.MaxSheild)).AddTo(this);
            _model.OnChangeHealth.Skip(1).Subscribe(h => _view.UpdateHpGauge(h / _model.MaxHealth)).AddTo(this);
            _model.OnDamage.Skip(1).Subscribe(d => _view.DamagePopUp(d)).AddTo(this);
            _model.OnHeal.Skip(1).Subscribe(h => _view.HealPopUp(h)).AddTo(this);
            _model.OnAttackTarget.Subscribe(v => _view.AttackEffect(v)).AddTo(this);
            _model.OnIsSelect.Subscribe(s => _view.IsSelect(s)).AddTo(this);
            return this;
        }

        public void BattleStart()
        {
            _model.BattleLoopStart();
        }


        public void SetGroup(CharacterUnitModel[] teamGroup, CharacterUnitModel[] enemyGroup)
        {
            _model.SetGroup(teamGroup,enemyGroup);
        }
    }
}
