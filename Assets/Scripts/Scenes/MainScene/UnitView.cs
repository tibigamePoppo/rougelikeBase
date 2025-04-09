using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Scenes.Battle;
using Scenes.EventScene;
using UnityEngine.EventSystems;

namespace Scenes.MainScene
{
    public class UnitView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        private Subject<bool> _isMouseOver = new Subject<bool>();

        public IObservable<EventUnit> OnClickEvent => _clickEvent;
        public IObservable<bool> IsPlayerWinBattle => _isPlayerWinBattle;
        public IObservable<bool> OnIsMouseOver => _isMouseOver;


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
                    PlayerSingleton.Instance.UpdateBattleReport(normalBattle: 1);
                    break;
                case UnitType.Boss:
                    SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                    _enemyLevel = EnemyLevel.Boss;
                    PlayerSingleton.Instance.UpdateBattleReport(bossBattle: 1);
                    break;
                case UnitType.Elite:
                    SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                    _enemyLevel = EnemyLevel.Elite;
                    PlayerSingleton.Instance.UpdateBattleReport(eliteBattle: 1);
                    break;
                case UnitType.Shop:
                    SceneManager.LoadScene("ShopScene", LoadSceneMode.Additive);
                    PlayerSingleton.Instance.UpdateBattleReport(shopUnit: 1);
                    break;
                case UnitType.Event:
                    SceneManager.LoadScene("EventScene", LoadSceneMode.Additive);
                    PlayerSingleton.Instance.UpdateBattleReport(eventUnit: 1);
                    break;
            }
            PlayerSingleton.Instance.UpdateBattleReport(depth: 1);
            _clickEvent.OnNext(eventUnit);
        }


        private int[] _stageDepth = { 0, 11, 24 };// base info is stageModel.cs
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FadeSceneSingleton.Instance.FadeOutEffet();
            if (scene.name == "BattleScene")
            {
                BattlePresenter battlePresenter = FindFirstObjectByType<BattlePresenter>();
                if (battlePresenter != null)
                {
                    int stageDepth = _unit.depth + _stageDepth[_unit.stageNumber];
                    battlePresenter.Init(_enemyLevel,PlayerSingleton.Instance.CurrentDeck, stageDepth, _unit.seed);
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
                eventPresenter.Init(_unit.depth, _unit.seed);
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private bool IsActiveOtherScene()
        {
            return Enumerable.Range(0, SceneManager.sceneCount)
                             .Select(SceneManager.GetSceneAt)
                             .Any(scene => scene.name == "BattleScene" || scene.name == "ShopScene" || scene.name == "EventScene");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isMouseOver.OnNext(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isMouseOver.OnNext(true);
        }

        public Transform imageTransform { get { return _image.transform; } }
    }
}