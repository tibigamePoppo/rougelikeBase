using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Scenes.MainScene.Player;

namespace Scenes.MainScene.Cards
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        public void Init(Card card)
        {
            _nameText.text = card.name;
            _image.sprite = card.sprite;
            _text.text = card.text;
        }
    }
}