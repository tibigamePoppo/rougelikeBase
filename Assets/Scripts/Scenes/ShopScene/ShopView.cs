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
        [SerializeField] private ShopRelicItemView _shopRelicItemView;
        [SerializeField] private Transform _itemPanel;
        [SerializeField] private Transform _relicItemPanel;
        private const int ITEMCOUNT = 7;
        private const int RELICITEMCOUNT = 3;
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
            var _relicDataList = Resources.Load<RelicItemPool>("Value/RelicItemPool").relicItem;
            for (int i = 0; i < RELICITEMCOUNT; i++)
            {
                var randomRelic = _relicDataList[Random.Range(0, _relicDataList.Length)];
                var shopRelicItem = Instantiate(_shopRelicItemView, _relicItemPanel);
                shopRelicItem.Init(randomRelic);
            }
        }

        private void BackMainScene()
        {
            SceneManager.UnloadSceneAsync("ShopScene");
        }
    }
}