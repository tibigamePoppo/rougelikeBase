using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Scenes.Battle;
using Scenes.EventScene;

namespace Scenes.MainScene
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField] private SpritePool _spritePool;
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        private EventUnit _unit;
        private CharacterIconView _iconView;
        public EventUnit eventUnit { get { return _unit; } }
        private EnemyLevel _enemyLevel;
        private const float RANDOMVALUEX = 0.5f;
        private const float RANDOMVALUEY = 0.2f;


        private Subject<EventUnit> _clickEvent = new Subject<EventUnit>();
        private Subject<bool> _isPlayerWinBattle = new Subject<bool>();

        public IObservable<EventUnit> OnClickEvent => _clickEvent;
        public IObservable<bool> IsPlayerWinBattle => _isPlayerWinBattle;


        public void Intaractable(bool value)
        {
            _button.interactable = value;
        }

        public void FadeUnit(bool value)
        {
            _image.color = value ? Color.gray : Color.white;
        }

        public void Init(EventUnit unit, CharacterIconView iconView)
        {
            _unit = unit;
            _iconView = iconView;
            _button.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => Click());
            _image.sprite = _spritePool.sprites.FirstOrDefault(v => v.name == _unit.unitType.ToString());
            if (unit.unitType != UnitType.Start && unit.unitType != UnitType.Boss)
            {
                _image.gameObject.transform.position += Vector3.right * UnityEngine.Random.Range(-RANDOMVALUEX, RANDOMVALUEX);
                _image.gameObject.transform.position += Vector3.up * UnityEngine.Random.Range(-RANDOMVALUEY, RANDOMVALUEY);
            }
        }

        private void Click()
        {
            Intaractable(false);
            _image.color = Color.gray;
            _iconView.UpdateIconPosition(transform);
            if (_unit.unitType != UnitType.Start) FadeSceneSingleton.Instance.FadeInEffet(() => SceneLoad());
            else _clickEvent.OnNext(eventUnit);
            
        }

        private void SceneLoad()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            switch (_unit.unitType)
            {
                case UnitType.Battle:
                    SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                    _enemyLevel = EnemyLevel.Normal;
                    break;
                case UnitType.Boss:
                    SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                    _enemyLevel = EnemyLevel.Boss;
                    break;
                case UnitType.Elite:
                    SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                    _enemyLevel = EnemyLevel.Elite;
                    break;
                case UnitType.Shop:
                    SceneManager.LoadScene("ShopScene", LoadSceneMode.Additive);
                    break;
                case UnitType.Event:
                    SceneManager.LoadScene("EventScene", LoadSceneMode.Additive);
                    break;
            }
            _clickEvent.OnNext(eventUnit);
        }


        private int[] _stageDepth = { 0, 11, 13 };// base info is stageModel.cs
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FadeSceneSingleton.Instance.FadeOutEffet();
            if (scene.name == "BattleScene")
            {
                BattlePresenter battlePresenter = FindFirstObjectByType<BattlePresenter>();
                if (battlePresenter != null)
                {
                    int stageDepth = _unit.depth + _stageDepth[_unit.stageNumber];
                    battlePresenter.Init(_enemyLevel,PlayerSingleton.Instance.CurrentDeck, stageDepth);
                }
                else
                {
                    Debug.LogWarning("BattlePresenter が見つからない！");
                }
                if(_enemyLevel == EnemyLevel.Boss)
                {
                    battlePresenter.IsPlayerWinBattle.Subscribe(isWin => _isPlayerWinBattle.OnNext(isWin)).AddTo(this);
                }
            }
            else if(scene.name == "EventScene")
            {
                EventScenePtesenter eventPresenter = FindFirstObjectByType<EventScenePtesenter>();
                eventPresenter.Init(_unit.depth);
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private bool IsActiveOtherScene()
        {
            return Enumerable.Range(0, SceneManager.sceneCount)
                             .Select(SceneManager.GetSceneAt)
                             .Any(scene => scene.name == "BattleScene" || scene.name == "ShopScene" || scene.name == "EventScene");
        }

        public Transform imageTransform { get { return _image.transform; } }
    }
}