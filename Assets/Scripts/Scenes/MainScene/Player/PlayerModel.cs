using System.Collections;
using System.Collections.Generic;
using UniRx;
using System;
using UnityEngine;
using Scenes.MainScene.Relic;

namespace Scenes.MainScene.Player
{
    public class PlayerModel
    {
        private IntReactiveProperty _popularity = new IntReactiveProperty(100);
        private IntReactiveProperty _money = new IntReactiveProperty(50);
        private Subject<Unit> _updateDeck = new Subject<Unit>();
        private List<UnitData> _cardDataList = new List<UnitData>();
        private List<RelicItemBase> _relicItems = new List<RelicItemBase>();
        private Subject<RelicItemBase[]> _updateRelicItem = new Subject<RelicItemBase[]>();

        public IObservable<int> OnPopularityChange => _popularity;
        public IObservable<int> OnMoneyChange => _money;
        public IObservable<Unit> OnDeckChange => _updateDeck;
        public IObservable<RelicItemBase[]> OnUpdateRelicItem => _updateRelicItem;
        public int CurrentPopularity { get { return _popularity.Value; } }
        public int CurrentMoney { get { return _money.Value; } }
        public List<UnitData> CurrentCardDataList { get { return _cardDataList; } }

        public void Init()
        {
            _cardDataList = Resources.Load<CardPool>("Value/PlayerDeck").CardList();
        }

        public void ChangePopularity(int value)
        {
            _popularity.Value = Mathf.Max(_popularity.Value + value,0);
        }

        public void ChangeMoney(int value)
        {
            _money.Value = Mathf.Max(_money.Value + value);
        }

        public bool CanTakeDamage(int value)
        {
            return _popularity.Value + value >= 0;
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

        public void RemoveCard(UnitData card)
        {
            _cardDataList.Remove(card);
            _updateDeck.OnNext(default);
        }

        public void AddRelicItem(RelicItemBase relicItem)
        {
            _relicItems.Add(relicItem);
            _updateRelicItem.OnNext(_relicItems.ToArray());
        }

        public void RemoveRelicItem(RelicItemBase relicItem)
        {
            _relicItems.Remove(relicItem);
            _updateRelicItem.OnNext(_relicItems.ToArray());
        }
    }
}
