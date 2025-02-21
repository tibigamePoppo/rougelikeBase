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
        [SerializeField] private CardPanelView _cardPanel;
        private List<CardPanelView> instanceCardList = new List<CardPanelView>();

        public void Init(List<CardData> playerCard)
        {
            _backButton.OnClickAsObservable().Subscribe(_ => gameObject.SetActive(false)).AddTo(this);

            Debug.Log($"playerCard {playerCard.Count}");
            foreach (var cardData in playerCard)
            {
                Debug.Log($"playerdata {cardData}");
                var card = Instantiate(_cardPanel, _content);
                card.Init(cardData);
                instanceCardList.Add(card);
            }
            gameObject.SetActive(false);
        }

        public void ActiveWindow(List<CardData> playerCard)
        {
            gameObject.SetActive(true);

            foreach (var cardData in playerCard)
            {
                instanceCardList.FirstOrDefault(c => c.Card.name == cardData.card.name).UpdateView(playerCard.Count);
            }
        }
    }
}