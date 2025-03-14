using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

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
            }
            var _relicDataList = Resources.Load<RelicItemPool>("Value/RelicItemPool").relicItem;
            for (int i = 0; i < RELICITEMCOUNT; i++)
            {
                var randomRelic = _relicDataList[Random.Range(0, _relicDataList.Length)];
                var shopRelicItem = Instantiate(_shopRelicItemView, _relicItemPanel);
                shopRelicItem.Init(randomRelic);
                shopItemViewList.Add(shopRelicItem);
            }

            foreach (var item in shopItemViewList)
            {
                item.OnBoughtEvent.Subscribe(_ =>
                {
                    UpdateCost();
                    if (!IsBuyableItem())
                    {
                        BackButtonAnimation();
                    }
                }).AddTo(this);
            }
            if (!IsBuyableItem())
            {
                BackButtonAnimation();
            }
        }

        private void BackButtonAnimation()
        {
            var currentScale = _backMainSceneButton.transform.localScale;
            _backMainSceneButton.transform.DOScale(currentScale * 1.2f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(0.2f);
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

        private bool IsBuyableItem()
        {
            return shopItemViewList.Any(item => item.shopCost <= PlayerSingleton.Instance.CurrentMoney);
        }
    }
}