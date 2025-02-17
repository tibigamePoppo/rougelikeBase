using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

namespace Scenes.Shopscene
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private Button _backMainSceneButton;
        public void Init()
        {
            _backMainSceneButton.OnClickAsObservable().Subscribe(_ => BackMainScene());
        }

        private void BackMainScene()
        {
            SceneManager.UnloadSceneAsync("ShopScene");
        }
    }
}