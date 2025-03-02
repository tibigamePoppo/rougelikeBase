using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Scenes.Battle;

namespace Scenes.EventScene
{
    public class EventSceneView : MonoBehaviour
    {
        [SerializeField] private EventSceneButtonView _button;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _eventImage;
        [SerializeField] private Transform _buttonPanel;
        private Subject<EventEffectArg> _emitEvent = new Subject<EventEffectArg>();
        public IObservable<EventEffectArg> OnEmitEvent => _emitEvent;
        private EnemyLevel _enemyLevel;

        public void Init(EventData eventData)
        {
            _text.text = eventData.text;
            _eventImage.sprite = eventData.sprite;
            foreach (var eventUnit in eventData.eventEffectArgs)
            {
                var button = Instantiate(_button, _buttonPanel);
                button.Init(eventUnit);
                button.Interactable(-eventUnit.playerMoneyChange < PlayerSingleton.Instance.CurrentMoney);
                button.OnClick.Subscribe(_ => _emitEvent.OnNext(eventUnit)).AddTo(this);
            }
        }

        public void ChangeScene(SceneName sceneName)
        {

            if (sceneName == SceneName.BattleScene)
            {
                _enemyLevel = EnemyLevel.Normal;
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Additive);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "BattleScene")
            {
                BattlePresenter battlePresenter = FindObjectOfType<BattlePresenter>();
                if (battlePresenter != null)
                {
                    battlePresenter.Init(_enemyLevel, PlayerSingleton.Instance.CurrentDeck);
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
