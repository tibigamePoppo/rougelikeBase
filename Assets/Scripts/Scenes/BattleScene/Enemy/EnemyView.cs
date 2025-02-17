using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Scenes.Battle.Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _name;

        public void Init(EnemyData data)
        {
            _name.text = data._enemyName;
            _image.sprite = data._enemySprite;
        }
    }
}