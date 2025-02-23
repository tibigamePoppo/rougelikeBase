using UnityEngine;
using TMPro;
using Scenes.MainScene.Player;

namespace Scenes.MainScene.Cards
{
    public class CardPanelView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CardView _view;
        private UnitStatus _cardData;
        public UnitStatus Card { get { return _cardData; } }

        public void Init(UnitData cardData,int unitCount)
        {
            _cardData = cardData.status;
            _view.Init(_cardData);
            UpdateView(unitCount);
        }

        public void UpdateView(int count)
        {
            if (count == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                _text.text = $"X {count}";
            }
        }
    }
}