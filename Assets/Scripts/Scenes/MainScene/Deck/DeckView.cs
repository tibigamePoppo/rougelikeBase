using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using Scenes.MainScene.Player;
using Scenes.MainScene.Cards;

namespace Scenes.MainScene.Decks
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Transform _content;
        [SerializeField] private CardView _card;
        private List<Card> _pastCard = new List<Card>();
        private List<CardView> instanceCardList = new List<CardView>();

        public void Init()
        {
            _backButton.OnClickAsObservable().Subscribe(_ => gameObject.SetActive(false)).AddTo(this);
            gameObject.SetActive(false);
        }

        public void ActiveWindow(List<Card> playerCard)
        {
            gameObject.SetActive(true);

            List<Card> added, removed;
            GetListDifferences(_pastCard, playerCard, out added, out removed);

            foreach (var diffCard in added)
            {
                var card = Instantiate(_card, _content);
                card.Init(diffCard);
                instanceCardList.Add(card);
            }
            foreach (var diffCard in removed)
            {
                CardView removeCard = instanceCardList.FirstOrDefault(c => c.Card.name == diffCard.name);
                //Destroy(removeCard.gameObject);// TODO 
            }
            _pastCard = new List<Card>(playerCard);
        }

        public void GetListDifferences(List<Card> A, List<Card> B, out List<Card> added, out List<Card> removed)
        {
            var aCounts = A.GroupBy(x => x.name).ToDictionary(g => g.Key, g => g.ToList());

            var bCounts = B.GroupBy(x => x.name).ToDictionary(g => g.Key, g => g.ToList());

            added = new List<Card>();
            removed = new List<Card>();

            // 追加されたカードのチェック
            foreach (var kvp in bCounts)
            {
                string name = kvp.Key;
                int countInB = kvp.Value.Count;
                int countInA = aCounts.ContainsKey(name) ? aCounts[name].Count : 0;
                Debug.Log($"{name} diff is {countInB - countInA}");
                if (countInB > countInA)
                {
                    added.AddRange(kvp.Value.Take(countInB - countInA));
                }
            }

            // 減ったカードのチェック
            foreach (var kvp in aCounts)
            {
                string name = kvp.Key;
                int countInA = kvp.Value.Count;
                int countInB = bCounts.ContainsKey(name) ? bCounts[name].Count : 0;

                if (countInA > countInB)
                {
                    removed.AddRange(kvp.Value.Take(countInA - countInB));
                }
            }
        }
    }
}