using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using Scenes.MainScene.Decks;

namespace Scenes.MainScene.Player
{
    public class PlayerUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _hpText;
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private Button _deckButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private DeckView _deckView;
        private Subject<Unit> _deckButtonClick = new Subject<Unit>();
        public IObservable<Unit> OnDeckButtonClick => _deckButtonClick;

        public void Init(int initialHp,int initialMoney, List<CardData> playerCard)
        {
            _hpText.text = initialHp.ToString();
            _moneyText.text = initialMoney.ToString();
            _deckView.Init(playerCard);
            _deckButton.OnClickAsObservable().Subscribe(_ => _deckButtonClick.OnNext(default)).AddTo(this);
            _menuButton.OnClickAsObservable().Subscribe(_ => MenuActive()).AddTo(this);
        }

        public void OpenDeckView(List<CardData> playerCard)
        {
            _deckView.ActiveWindow(playerCard);
        }

        public void UpdateHpText(int newHp)
        {
            _hpText.text = newHp.ToString();
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