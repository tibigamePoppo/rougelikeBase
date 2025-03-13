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
        private Button _button;
        private Image _image;
        private EventUnit _unit;
        private CharacterIconView _iconView;
        public EventUnit eventUnit { get { return _unit; } }
        private EnemyLevel _enemyLevel;


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
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            _button.OnClickAsObservable().Subscribe(_ => Click());
            _image.sprite = _spritePool.sprites.FirstOrDefault(v => v.name == _unit.unitType.ToString());
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

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FadeSceneSingleton.Instance.FadeOutEffet();
            if (scene.name == "BattleScene")
            {
                BattlePresenter battlePresenter = FindFirstObjectByType<BattlePresenter>();
                if (battlePresenter != null)
                {
                    battlePresenter.Init(_enemyLevel,PlayerSingleton.Instance.CurrentDeck, _unit.depth);
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
    }
}