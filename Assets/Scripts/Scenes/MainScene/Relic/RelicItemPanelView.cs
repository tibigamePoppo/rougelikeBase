using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Scenes.MainScene.Relic
{
    public class RelicItemPanelView : MonoBehaviour
    {

        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _text;
        public GameObject icon { get { return _icon.gameObject; } }

        public void Init(RelicItemBase relic)
        {
            _icon.sprite = relic.sprite;
            _name.text = relic.relicItemName;
            _text.text = relic.effectText;
        }
    }
}