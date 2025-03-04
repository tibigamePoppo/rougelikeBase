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
        [SerializeField] private TextMeshProUGUI _hpText;
        [SerializeField] private TextMeshProUGUI _attackText;
        [SerializeField] private TextMeshProUGUI _speedText;
        [SerializeField] private TextMeshProUGUI _rangeText;
        [SerializeField] private TextMeshProUGUI _text;

        public void Init(UnitStatus unit)
        {
            _nameText.text = unit.name;
            _image.sprite = unit.sprite;
            _hpText.text = unit.hp.ToString();
            _attackText.text = unit.attack.ToString();
            _speedText.text = unit.speed.ToString();
            _rangeText.text = unit.attackRange.ToString();
            _text.text = unit.text;
        }
    }
}