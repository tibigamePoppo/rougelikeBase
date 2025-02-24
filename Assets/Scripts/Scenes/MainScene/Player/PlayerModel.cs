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
        private Subject<Unit> _updateDeck = new Subject<Unit>();
        private List<UnitData> _cardDataList = new List<UnitData>();

        public IObservable<int> OnHpChange => _hp;
        public IObservable<int> OnMoneyChange => _money;
        public IObservable<Unit> OnDeckChange => _updateDeck;
        public int CurrentHp { get { return _hp.Value; } }
        public int CurrentMoney { get { return _money.Value; } }
        public List<UnitData> CurrentCardDataList { get { return _cardDataList; } }

        public void Init()
        {
            _cardDataList = Resources.Load<CardPool>("Value/PlayerDeck").CardList();
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

        public void AddCard(UnitData card)
        {
            _cardDataList.Add(card);
            _updateDeck.OnNext(default);
        }
    }
}
