using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

namespace Scenes.Battle.UnitCharacter
{
    public class CharacterUnitView : MonoBehaviour
    {
        [SerializeField] private Image _hpGauge;
        [SerializeField] private Image _shieldGauge;
        [SerializeField] private EffectEmitBase[] _attackEffect;
        [SerializeField] private TextMeshProUGUI _damagePopup;
        [SerializeField] private Transform _popupPosition;
        [SerializeField] private Transform _lookCamera;

        private Animator _animator;
        public Animator Animator { get { return _animator; } }
        private int _animIDSpeed;
        private Camera sceneCamera;

        private Subject<Vector3> formationPoint = new Subject<Vector3>();
        public IObservable<Vector3> OnMoveFormationPoint => formationPoint;

        public void Init(NavMeshAgent agent )
        {
            _animator = GetComponent<Animator>();
            _animIDSpeed = Animator.StringToHash("Speed");

            Scene currentScene = gameObject.scene;
            sceneCamera = GetCameraInScene(currentScene);
            this.UpdateAsObservable().Subscribe(_ => {
                _animator.SetFloat(_animIDSpeed, agent.velocity.magnitude, 0.15f, Time.deltaTime);
                LookCamera();
            }).AddTo(this);

        }

        private void LookCamera()
        {
            Vector3 direction = sceneCamera.transform.position - transform.position;
            direction.x = 0;
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                _lookCamera.transform.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 180);
            }
        }

        public void UpdateHpGauge(float fillValue)
        {
            if (fillValue >= 0)
            {
                _hpGauge.fillAmount = fillValue;
            }
        }

        public void UpdateSheildGauge(float fillValue)
        {
            if (fillValue >= 0)
            {
                _shieldGauge.fillAmount = fillValue;
            }
        }

        public void DamagePopUp(float value)
        {
            var popupText = Instantiate(_damagePopup, _popupPosition);
            popupText.text = value.ToString();
            popupText.color = Color.red;
        }
        public void HealPopUp(float value)
        {
            var popupText = Instantiate(_damagePopup, _popupPosition);
            popupText.text = value.ToString();
            popupText.color = new Color(0.72f, 0.82f, 0);
        }

        public void AttackEffect(AttackArg arg)
        {
            var effectNum = arg.effectNum >= _attackEffect.Length ? 0: arg.effectNum;
            _attackEffect[effectNum].Emit(arg.target);
        }
        private void OnAttackEffect(AnimationEvent animationEvent)
        {
            //_attackEffect.Emit(Vector3.zero);
        }

        public void HideHpGauge()
        {
            _lookCamera.gameObject.SetActive(false);
        }

        Camera GetCameraInScene(Scene scene)
        {
            Camera[] camerasInScene = scene.GetRootGameObjects()
                                            .SelectMany(go => go.GetComponentsInChildren<Camera>())
                                            .ToArray();
            return camerasInScene.Length > 0 ? camerasInScene.First() : null;
        }
    }
}