using UnityEngine;
using System.Linq;

namespace Scenes.MainScene.Relic.Item
{
    public class UpgaredeKnight : RelicItemBase
    {
        public override void Init()
        {
            var playerDeck = PlayerSingleton.Instance.CurrentDeck;
            var Sowrdman = Resources.Load<CardPool>("Value/PlayerAllUnitPool").cards.FirstOrDefault(c => c.status.name == "歩兵");
            var HorceKnight = Resources.Load<CardPool>("Value/PlayerAllUnitPool").cards.FirstOrDefault(c => c.status.name == "騎兵");
            var SowrdmanCount = playerDeck.Count(d => d == Sowrdman);
            for (int i = 0; i < SowrdmanCount; i++)
            {
                PlayerSingleton.Instance.RemoveCard(Sowrdman);
                PlayerSingleton.Instance.AddCard(HorceKnight);
            }
        }
    }
}