using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

namespace Scenes.Shopscene
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private Button _backMainSceneButton;
        [SerializeField] private ShopItemView _shopItemView;
        [SerializeField] private Transform _itemPanel;
        private const int ITEMCOUNT = 7;
        public void Init()
        {
            _backMainSceneButton.OnClickAsObservable().Subscribe(_ => BackMainScene());
            var _cardDataList = Resources.Load<CardPool>("Value/ShopUnitPool").CardList();
            for (int i = 0; i < ITEMCOUNT; i++)
            {
                var randomUnit = _cardDataList[Random.Range(0, _cardDataList.Count)];
                var shopItem = Instantiate(_shopItemView, _itemPanel);
                shopItem.Init(randomUnit);
            }
        }

        private void BackMainScene()
        {
            SceneManager.UnloadSceneAsync("ShopScene");
        }
    }
}