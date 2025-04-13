using UnityEngine;
using UnityEngine.UI;

namespace Scenes.MainScene.Player
{
    public class ProgressSingleView : MonoBehaviour
    {
        [SerializeField] private Sprite _progressed;
        [SerializeField] private Sprite[] _defaultSprite;
        private Image _currentImage;
        public void Init(ProgressType type)
        {
            _currentImage = GetComponent<Image>();
            _currentImage.sprite = _defaultSprite[(int)type];
        }

        public void Progressed()
        {
            _currentImage.sprite = _progressed;
        }
    }

    public enum ProgressType
    {
        Start = 0,
        Normal = 1,
        Boss = 2,
    }
}