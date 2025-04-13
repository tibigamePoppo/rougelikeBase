using UnityEngine;
using UnityEngine.UI;

namespace Scenes.MainScene.Player
{
    public class ProgressSingleView : MonoBehaviour
    {
        [SerializeField] private Sprite[] _defaultSprite;
        [SerializeField] private GameObject _nextMarker;
        private Image _currentImage;
        public void Init(ProgressType type)
        {
            _nextMarker.SetActive(false);
            _currentImage = GetComponent<Image>();
            _currentImage.sprite = _defaultSprite[(int)type];
        }

        public void NextProgress()
        {
            _nextMarker.SetActive(true);
        }

        public void Progressed()
        {
            _nextMarker.SetActive(false);
            _currentImage.color = Color.gray;
        }
    }

    public enum ProgressType
    {
        Start = 0,
        Normal = 1,
        Boss = 2,
    }
}