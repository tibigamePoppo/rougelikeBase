using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.EventScene
{
    public class EventSceneModel
    {
        private EventBase[] _sceneEvent;
        public EventBase[] SceneEvent { get { return _sceneEvent; } }
        public void Init()
        {
            _sceneEvent = RandomEventList();
        }

        private EventBase[] RandomEventList()
        {
            List<EventBase> events = new List<EventBase>();
            int randomIndex = Random.RandomRange(0, 4);
            switch (randomIndex)
            {
                case 0:
                    events.Add(new EventBase("big heal portion", new EventEffectArg(100, -30, "")));
                    events.Add(new EventBase("small heal portion", new EventEffectArg(50, -10, "")));
                    events.Add(new EventBase("harb only", new EventEffectArg(10, 0, "")));
                    break;
                case 1:
                    events.Add(new EventBase("Fighting the Bandits", new EventEffectArg(0, 0, "BattleScene")));
                    events.Add(new EventBase("give money.", new EventEffectArg(0, -10, "")));
                    break;
                case 2:
                    events.Add(new EventBase("rest", new EventEffectArg(30, 0, "")));
                    events.Add(new EventBase("work.", new EventEffectArg(0, 30, "")));
                    break;
                case 3:
                    events.Add(new EventBase("go shop", new EventEffectArg(0, 0, "ShopScene")));
                    events.Add(new EventBase("back home.", new EventEffectArg(0, 30, "")));
                    break;
                default:
                    break;
            }
            return events.ToArray();
        }

        public void EventProcess(EventEffectArg arg)
        {
            Debug.Log($"change hp {arg.playerHpChange}");
            PlayerSingleton.Instance.ChangeHp(arg.playerHpChange);
            Debug.Log($"change money {arg.playerMoneyChange}");
            PlayerSingleton.Instance.ChangeMoney(arg.playerMoneyChange);
            Debug.Log($"change scene {arg.changeScene}");
            if (arg.changeScene.Length > 0)
            {
                SceneManager.LoadScene(arg.changeScene, LoadSceneMode.Additive);
            }
            SceneManager.UnloadSceneAsync("EventScene");
        }
    }

    public struct EventEffectArg
    {
        public int playerHpChange;
        public int playerMoneyChange;
        public string changeScene;
        public EventEffectArg(int playerHpChange, int playerMoneyChange, string changeScene)
        {
            this.playerHpChange = playerHpChange;
            this.playerMoneyChange = playerMoneyChange;
            this.changeScene = changeScene;
        }
    }
}