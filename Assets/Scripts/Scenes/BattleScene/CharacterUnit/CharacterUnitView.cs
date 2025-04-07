using UnityEngine;
using UniRx;
using System;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Linq;
using Random = UnityEngine.Random;

namespace Scenes.Battle.UnitCharacter
{
    public class CharacterUnitView : MonoBehaviour
    {
        [SerializeField] private Image _hpGauge;
        [SerializeField] private Image _shieldGauge;
        [SerializeField] private GameObject _groupColorCircle;
        [SerializeField] private GameObject _footStepPrefab;
        [SerializeField] private EffectEmitBase[] _attackEffect;
        [SerializeField] private DamagePopUp _damagePopup;
        [SerializeField] private Transform _popupPosition;
        [SerializeField] private Transform _lookCamera;
        [SerializeField] private QuiqkOutline _quiqkOutline;
        [SerializeField] private GameObject _moveMarker;
        [SerializeField] private LineRenderer _moveLineRenderer;
        private bool _isDisplayMoveMarker = false;
        private bool _isDead = false;

        private Vector3 _randomPupupOffset = new Vector3(0.8f, 0.8f, 0);
        private BoxCollider _collider;
        private NavMeshAgent _agent;
        private Color NORMAL_OUTLINE_COLOR = Color.black;
        private const int NORMAL_OUTLINE_WIDTH = 1;
        private Color SELECT_OUTLINE_COLOR = Color.yellow;
        private const int SELECT_OUTLINE_WIDTH = 6;

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
            _moveMarker.SetActive(false);
            if (_quiqkOutline != null)
            {
                _quiqkOutline.OutlineWidth = NORMAL_OUTLINE_WIDTH;
                _quiqkOutline.OutlineColor = NORMAL_OUTLINE_COLOR;
            }

            Scene currentScene = gameObject.scene;
            sceneCamera = GetCameraInScene(currentScene);
            this.UpdateAsObservable().Subscribe(_ => {
                _animator.SetFloat(_animIDSpeed, agent.velocity.magnitude, 0.15f, Time.deltaTime);
                LookCamera();
            }).AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => _isDisplayMoveMarker)
                .Subscribe(_ =>
                {
                    if(_agent.isActiveAndEnabled)
                    { 
                        NavMeshPath path = _agent.path;
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            _moveLineRenderer.positionCount = path.corners.Length;
                            _moveLineRenderer.SetPosition(0, Vector3.zero);
                            for (int i = 1; i <= path.corners.Length; i++)
                            {
                                _moveLineRenderer.SetPosition(i - 1, path.corners[i - 1] - transform.position);
                            }
                            _moveMarker.transform.position = _agent.destination;
                            _moveLineRenderer.transform.rotation = Quaternion.identity;
                        }
                        else
                        {
                            _moveLineRenderer.positionCount = 0;
                        }
                    }
                    else
                    {
                        _moveLineRenderer.positionCount = 0;
                    }
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
            popupText.Init(value);
        }
        public void HealPopUp(float value)
        {
            var popupText = Instantiate(_damagePopup, _popupPosition);
            DamageArg arg = new DamageArg(value, new Color(0.4352f, 0.9255f, 0.4667f), DamageAnimation.Jump);
            popupText.Init(arg);
        }

        public void AttackEffect(AttackArg arg)
        {
            var effectNum = arg.effectNum >= _attackEffect.Length ? 0: arg.effectNum;
            _attackEffect[effectNum].Emit(arg.target);
            if(_isDisplayMoveMarker)
            {
                _moveMarker.transform.position = arg.target;
                _moveLineRenderer.positionCount = 2;
                _moveLineRenderer.SetPosition(0, Vector3.zero);
                _moveLineRenderer.SetPosition(1, arg.target - transform.position);
                _moveLineRenderer.transform.rotation = Quaternion.identity;
            }
        }
        private void OnAttackEffect(AnimationEvent animationEvent)
        {
            //_attackEffect.Emit(Vector3.zero);
        }

        private void FootStep(AnimationEvent animationEvent)
        {
            if (_footStepPrefab == null) return;
            var footStep = Instantiate(_footStepPrefab, transform);
            Destroy(footStep, 2);
        }

        public void HideHpGauge()
        {
            _lookCamera.gameObject.SetActive(false);
        }

        public void HideGroupColorCircle()
        {
            _groupColorCircle.SetActive(false);
            _isDead = true;
            _moveMarker.SetActive(false);
            _moveLineRenderer.gameObject.SetActive(false);
            if (_quiqkOutline != null)
            {
                _quiqkOutline.OutlineWidth = NORMAL_OUTLINE_WIDTH;
                _quiqkOutline.OutlineColor = NORMAL_OUTLINE_COLOR;
            }
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

        public void IsSelect(bool value )
        {
            if (_quiqkOutline == null && !_isDead) return; 
            _quiqkOutline.OutlineWidth = value ? SELECT_OUTLINE_WIDTH : NORMAL_OUTLINE_WIDTH;
            _quiqkOutline.OutlineColor = value ? SELECT_OUTLINE_COLOR : NORMAL_OUTLINE_COLOR;
            _moveMarker.SetActive(value);
            _moveLineRenderer.gameObject.SetActive(value);
            _isDisplayMoveMarker = value;
        }
    }
}