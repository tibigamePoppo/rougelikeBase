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

        public void Init(UnitStatus unit)
        {
            _nameText.text = unit.name;
            _image.sprite = unit.sprite;
            _text.text = unit.text;
        }
    }
}