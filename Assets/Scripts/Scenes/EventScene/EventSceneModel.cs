using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System;

namespace Scenes.EventScene
{
    public class EventSceneModel
    {
        private EventData _sceneEvent;
        private Subject<SceneName> _changeScene = new Subject<SceneName>();
        public EventData SceneEvent { get { return _sceneEvent; } }
        public IObservable<SceneName> ChangeScene => _changeScene;

        public void Init()
        {
            _sceneEvent = RandomEventList();
        }

        private EventData RandomEventList()
        {
            List<EventBase> events = new List<EventBase>();
            var _eventDataList = Resources.Load<EventDataPool>("Value/EventPool").events.ToArray();
            int randomIndex = UnityEngine.Random.Range(0, _eventDataList.Length);
            return _eventDataList[randomIndex];
        }

        public void EventProcess(EventEffectArg arg)
        {
            Debug.Log($"change hp {arg.playerHpChange}");
            PlayerSingleton.Instance.ChangeHp(arg.playerHpChange);
            Debug.Log($"change money {arg.playerMoneyChange}");
            PlayerSingleton.Instance.ChangeMoney(arg.playerMoneyChange);
            Debug.Log($"change scene {arg.changeScene}");
            if (arg.changeScene != SceneName.None)
            {
                _changeScene.OnNext(arg.changeScene);
            }
            foreach (var unit in arg.units)
            {
                Debug.Log($"add member {unit.name}");
                PlayerSingleton.Instance.AddCard(unit);
            }
            SceneManager.UnloadSceneAsync("EventScene");
        }

    }
}