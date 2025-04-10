using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Scenes.Battle;

namespace Scenes.MainScene
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField] private SpritePool _spritePool;
        private Button _button;
        private Image _image;
        private EventUnit _unit;
        private CharacterIconView _iconView;
        private Subject<int> _clickEvent = new Subject<int>();
        public EventUnit eventUnit { get { return _unit; } }
        public IObservable<int> OnClickEvent => _clickEvent;
        private EnemyLevel _enemyLevel;

        public void Intaractable(bool value)
        {
            _button.interactable = value;
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
            switch (_unit.unitType)
            {
                case UnitType.Battle:
                    SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                    _enemyLevel = EnemyLevel.Normal;
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    break;
                case UnitType.Boss:
                    SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                    _enemyLevel = EnemyLevel.Boss;
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    break;
                case UnitType.Elite:
                    SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                    _enemyLevel = EnemyLevel.Elite;
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    break;
                case UnitType.Shop:
                    SceneManager.LoadScene("ShopScene", LoadSceneMode.Additive);
                    break;
                case UnitType.Event:
                    SceneManager.LoadScene("EventScene", LoadSceneMode.Additive);
                    break;

            }
            _clickEvent.OnNext(eventUnit.depth);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "BattleScene")
            {
                BattlePresenter battlePresenter = FindObjectOfType<BattlePresenter>();
                if (battlePresenter != null)
                {
                    battlePresenter.Init(_enemyLevel);
                    Debug.Log("BattlePresenter の Init を実行！");
                }
                else
                {
                    Debug.LogWarning("BattlePresenter が見つからない！");
                }

                // イベント登録解除（不要なら消す）
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
    }
}