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
        private Subject<EventEffectArg> _changeScene = new Subject<EventEffectArg>();
        public EventData SceneEvent { get { return _sceneEvent; } }
        public IObservable<EventEffectArg> ChangeScene => _changeScene;

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
            Debug.Log($"change Popularity {arg.playerPopularityChange}");
            PlayerSingleton.Instance.ChangePopularity(arg.playerPopularityChange);
            Debug.Log($"change money {arg.playerMoneyChange}");
            PlayerSingleton.Instance.ChangeMoney(arg.playerMoneyChange);
            Debug.Log($"change scene {arg.changeScene}");
            if(arg.relic != null)
            {
                arg.relic.Init();
                Debug.Log($"Reric {arg.relic.name}");
            }

            if (arg.changeScene != SceneName.None)
            {
                _changeScene.OnNext(arg);
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