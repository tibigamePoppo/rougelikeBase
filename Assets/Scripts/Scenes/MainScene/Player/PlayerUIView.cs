using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using Scenes.MainScene.Decks;
using Scenes.MainScene.Upgrade;
using Scenes.MainScene.Relic;

namespace Scenes.MainScene.Player
{
    public class PlayerUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _popularityText;
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private Button _deckButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private DeckView _deckView;
        [SerializeField] private UnitUpgradePresenter _unitUpgradePresenter;
        private Subject<Unit> _deckButtonClick = new Subject<Unit>();
        public IObservable<Unit> OnDeckButtonClick => _deckButtonClick;

        public void Init(int initialPopularity, int initialMoney, List<UnitData> playerCard)
        {
            _popularityText.text = initialPopularity.ToString();
            _moneyText.text = initialMoney.ToString();
            _deckView.Init(playerCard);
            _unitUpgradePresenter.Init();
            _deckButton.OnClickAsObservable().Subscribe(_ => _deckButtonClick.OnNext(default)).AddTo(this);
            _menuButton.OnClickAsObservable().Subscribe(_ => MenuActive()).AddTo(this);
        }

        public void OpenDeckView(List<UnitData> playerCard, List<RelicItemBase> relicItems)
        {
            _deckView.ActiveWindow(playerCard,relicItems);
        }

        public void UpdatePopularityText(int newPopularity)
        {
            _popularityText.text = newPopularity.ToString();
        }

        public void UpdateMoneyText(int newMoney)
        {
            _moneyText.text = newMoney.ToString();
        }

        private void MenuActive()
        {
            if(!SceneManager.GetSceneByName("MenuScene").isLoaded)
            {
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Additive);
            }
        }
    }
}