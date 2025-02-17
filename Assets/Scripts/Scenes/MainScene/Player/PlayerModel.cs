using System.Collections;
using System.Collections.Generic;
using UniRx;
using System;
using UnityEngine;

namespace Scenes.MainScene.Player
{
    public class PlayerModel
    {
        private IntReactiveProperty _hp = new IntReactiveProperty(100);
        private IntReactiveProperty _money = new IntReactiveProperty(50);
        private List<Card> _cards = new List<Card>();

        public IObservable<int> OnHpChange => _hp;
        public IObservable<int> OnMoneyChange => _money;
        public int CurrentHp { get { return _hp.Value; } }
        public int CurrentMoney { get { return _money.Value; } }
        public List<Card> CurrentCards { get { return _cards; } }

        public void Init()
        {
            _cards = Resources.Load<CardPool>("Value/PlayerDeck").cards;
        }

        public void ChangeHp(int value)
        {
            _hp.Value = Mathf.Max(_hp.Value + value,0);
        }

        public void ChangeMoney(int value)
        {
            _money.Value = Mathf.Max(_money.Value + value);
        }

        public bool CanTakeDamage(int value)
        {
            return _hp.Value + value >= 0;
        }

        public bool CanTakeMoney(int value)
        {
            return _money.Value + value >= 0;
        }

        public void AddCard(Card card)
        {
            Debug.Log($"addCard name is {card.name}");
            _cards.Add(card);
        }
    }
}
