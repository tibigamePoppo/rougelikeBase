using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Scenes.Shopscene
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private Button _backMainSceneButton;
        [SerializeField] private ShopUnitItemView _shopUnitItemView;
        [SerializeField] private ShopRelicItemView _shopRelicItemView;
        [SerializeField] private Transform _itemPanel;
        [SerializeField] private Transform _relicItemPanel;
        private List<ShopItemView> shopItemViewList = new List<ShopItemView>();
        private const int ITEMCOUNT = 7;
        private const int RELICITEMCOUNT = 3;
        public void Init()
        {
            _backMainSceneButton.OnClickAsObservable().Subscribe(_ => BackMainScene());
            var _cardDataList = Resources.Load<CardPool>("Value/ShopUnitPool").CardList();
            for (int i = 0; i < ITEMCOUNT; i++)
            {
                var randomUnit = _cardDataList[Random.Range(0, _cardDataList.Count)];
                var shopItem = Instantiate(_shopUnitItemView, _itemPanel);
                shopItem.Init(randomUnit);
                shopItemViewList.Add(shopItem);
                shopItem.OnBoughtEvent.Subscribe(_ => UpdateCost()).AddTo(this);
            }
            var _relicDataList = Resources.Load<RelicItemPool>("Value/RelicItemPool").relicItem;
            for (int i = 0; i < RELICITEMCOUNT; i++)
            {
                var randomRelic = _relicDataList[Random.Range(0, _relicDataList.Length)];
                var shopRelicItem = Instantiate(_shopRelicItemView, _relicItemPanel);
                shopRelicItem.Init(randomRelic);
                shopItemViewList.Add(shopRelicItem);
                shopRelicItem.OnBoughtEvent.Subscribe(_ => UpdateCost()).AddTo(this);
            }
        }

        private void BackMainScene()
        {
            SceneManager.UnloadSceneAsync("ShopScene");
        }

        private void UpdateCost()
        {
            foreach (var item in shopItemViewList)
            {
                item.UpdateText();
            }
        }
    }
}