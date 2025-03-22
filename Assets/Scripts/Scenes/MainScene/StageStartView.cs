using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Scenes.MainScene
{
    public class StageStartView : MonoBehaviour
    {
        [SerializeField] private Button _startPanelEndButton;
        public void Init()
        {
            gameObject.SetActive(true);
            _startPanelEndButton.OnClickAsObservable().Subscribe(_ => gameObject.SetActive(false));
        }
    }
}