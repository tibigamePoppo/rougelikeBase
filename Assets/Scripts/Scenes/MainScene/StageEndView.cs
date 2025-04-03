using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

namespace Scenes.MainScene
{
    public class StageEndView : MonoBehaviour
    {
        [SerializeField] private Sprite _clearSprite;
        [SerializeField] private Sprite _defeatSprite;
        [SerializeField] private Image _backImage;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _titleButton;
        [SerializeField] private StageEndScoreView _stageEndScoreView;

        public void Init()
        {
            gameObject.SetActive(false);
            _retryButton.OnClickAsObservable().Subscribe(_ => SceneManager.LoadScene(SceneManager.GetActiveScene().name)).AddTo(this);
            _titleButton.OnClickAsObservable().Subscribe(_ => SceneManager.LoadScene("Title")).AddTo(this);
        }

        public void ActiveWindow(EndType type)
        {
            gameObject.SetActive(true);
            _stageEndScoreView.Init();
            switch (type)
            {
                case EndType.Win:
                    _backImage.sprite = _clearSprite;
                    break;
                case EndType.Defeat:
                    _backImage.sprite = _defeatSprite;
                    break;
                default:
                    break;
            }
        }
    }

    public enum EndType
    {
        Win,
        Defeat,
    }
}