using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System;
using System.Linq;

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
            _eventDataList = LimitFilterEvent(_eventDataList);
            int randomIndex = UnityEngine.Random.Range(0, _eventDataList.Length);
            return _eventDataList[randomIndex];
        }

        private EventData[] LimitFilterEvent(EventData[] events)
        {
            var data = events.Where(e =>
                                    e.limit.underLimitMoney <= PlayerSingleton.Instance.CurrentMoney &&
                                    (e.limit.upperLimitMoney == 0 || e.limit.upperLimitMoney >= PlayerSingleton.Instance.CurrentMoney) &&
                                    e.limit.underLimitPopularity <= PlayerSingleton.Instance.CurrentPopularity &&
                                    (e.limit.upperLimitPopularity == 0 || e.limit.upperLimitPopularity >= PlayerSingleton.Instance.CurrentPopularity) &&
                                    (e.limit.containRelic.Length == 0 || e.limit.containRelic.All(relic => PlayerSingleton.Instance.CurrentRelic.Contains(relic))) &&
                                    (e.limit.uncontainRelic.Length == 0 || !e.limit.uncontainRelic.Any(relic => PlayerSingleton.Instance.CurrentRelic.Contains(relic))) &&
                                    (e.limit.containUnits.Length == 0 || e.limit.containUnits.All(unit => PlayerSingleton.Instance.CurrentDeck.Contains(unit))) &&
                                    (e.limit.uncontainUnits.Length == 0 || !e.limit.uncontainUnits.Any(unit => PlayerSingleton.Instance.CurrentDeck.Contains(unit))) &&
                                    (e.limit.passEvent.Length == 0 || e.limit.passEvent.All(eventName => PlayerSingleton.Instance.CurrentPassEvent.Contains(eventName))) &&
                                    (e.limit.notPassEvent.Length == 0 || !e.limit.notPassEvent.Any(eventName => PlayerSingleton.Instance.CurrentPassEvent.Contains(eventName))) 
                                ).ToArray();
            return data;
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