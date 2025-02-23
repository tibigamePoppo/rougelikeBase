using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.AI;

namespace Scenes.Battle.UnitCharacter
{
    public class CharacterUnitView : MonoBehaviour
    {
        [SerializeField] private Image _hpGauge;

        private Animator _animator;
        public Animator Animator { get { return _animator; } }
        private int _animIDSpeed;

        public void Init(NavMeshAgent agent )
        {
            _animator = GetComponent<Animator>();
            _animIDSpeed = Animator.StringToHash("Speed");

            this.UpdateAsObservable().Subscribe(_ => {
                _animator.SetFloat(_animIDSpeed, agent.speed, 0.15f, Time.deltaTime);
            }).AddTo(this);

        }

        public void UpdateHpGauge(float fillValue)
        {
            _hpGauge.fillAmount = fillValue;
        }
    }
}