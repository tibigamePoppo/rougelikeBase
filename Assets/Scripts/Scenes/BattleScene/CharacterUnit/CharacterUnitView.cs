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
using Random = UnityEngine.Random;

namespace Scenes.Battle.UnitCharacter
{
    public class CharacterUnitView : MonoBehaviour
    {
        [SerializeField] private Image _hpGauge;
        [SerializeField] private Image _shieldGauge;
        [SerializeField] private GameObject _groupColorCircle;
        [SerializeField] private EffectEmitBase[] _attackEffect;
        [SerializeField] private TextMeshProUGUI _damagePopup;
        [SerializeField] private Transform _popupPosition;
        [SerializeField] private Transform _lookCamera;
        private Vector3 _randomPupupOffset = new Vector3(0.5f, 0.5f, 0);
        private BoxCollider _collider;
        private NavMeshAgent _agent;

        private Animator _animator;
        public Animator Animator { get { return _animator; } }
        private int _animIDSpeed;
        private Camera sceneCamera;

        private Subject<Vector3> formationPoint = new Subject<Vector3>();
        public IObservable<Vector3> OnMoveFormationPoint => formationPoint;

        public void Init(NavMeshAgent agent )
        {
            _agent = agent;
            _animator = GetComponent<Animator>();
            _animIDSpeed = Animator.StringToHash("Speed");
            _collider = GetComponent<BoxCollider>();

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

        public void DamagePopUp(DamageArg value)
        {
            Vector3 noise = new Vector3(Random.Range(-_randomPupupOffset.x, _randomPupupOffset.x),Random.Range(-_randomPupupOffset.y, _randomPupupOffset.y), Random.Range(-_randomPupupOffset.z, _randomPupupOffset.z));
            var popupText = Instantiate(_damagePopup, _popupPosition.transform.position + noise, _popupPosition.rotation, _popupPosition);
            popupText.text = value.damage.ToString();
            popupText.color = value.color;
        }
        public void HealPopUp(float value)
        {
            var popupText = Instantiate(_damagePopup, _popupPosition);
            popupText.text = value.ToString();
            popupText.color = new Color(0.4352f, 0.9255f, 0.4667f);
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

        public void HideGroupColorCircle()
        {
            _groupColorCircle.SetActive(false);
        }

        public void ColliderActive(bool value)
        {
            _collider.enabled = value;
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