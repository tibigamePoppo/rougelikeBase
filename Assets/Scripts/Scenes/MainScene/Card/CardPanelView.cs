using UnityEngine;
using TMPro;
using Scenes.MainScene.Player;

namespace Scenes.MainScene.Cards
{
    public class CardPanelView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CardView _view;
        private Card _cardData;
        public Card Card { get { return _cardData; } }

        public void Init(CardData cardData)
        {
            _cardData = cardData.card;
            _view.Init(_cardData);
            UpdateView(cardData.count);
        }

        public void UpdateView(int count)
        {
            if (count == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                _text.text = $"X {count}";
                gameObject.SetActive(true);
            }
        }
    }
}