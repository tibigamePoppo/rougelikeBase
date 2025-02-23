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

            _stateController.Init(_model,_view);

            _model.OnChangeHealth.Subscribe(h => _view.UpdateHpGauge(h/_model.MaxHealth)).AddTo(this);
            return this;
        }

        public void SetGroup(CharacterUnitModel[] teamGroup, CharacterUnitModel[] enemyGroup)
        {
            _model.SetGroup(teamGroup,enemyGroup);
        }
    }
}
