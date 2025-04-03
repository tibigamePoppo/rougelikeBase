using Common;
using UniRx;
using System;
using Scenes.MainScene.Player;
using System.Collections.Generic;
using Scenes.MainScene.Relic;
using UnityEngine;

public class PlayerSingleton : SingletonAbstract<PlayerSingleton>
{
    private Subject<int> _changePopularity = new Subject<int>();
    private Subject<int> _changeMoney = new Subject<int>();
    private Subject<UnitData> _addUnit = new Subject<UnitData>();
    private Subject<UnitData> _removeUnit = new Subject<UnitData>();
    private Subject<RelicItemBase> _addRelicItem = new Subject<RelicItemBase>();
    private Subject<RelicItemBase> _removeRelicItem = new Subject<RelicItemBase>();
    private Subject<BattleReportStruct> _battleReport = new Subject<BattleReportStruct>();
    private Subject<string> _addPassEventName = new Subject<string>();
    private List<UnitData> _currentDeck = new List<UnitData>();
    private RelicItemBase[] _currentRelicItem = new RelicItemBase[0];
    private string[] _currentPassEvent = new string[0];
    private int _playerMoney;
    private int _playerPopularity;
    private BattleReportStruct _battleReportStruct;

    public IObservable<int> OnChangePopularityEvent => _changePopularity;
    public IObservable<int> OnChangeMoneyEvent => _changeMoney;
    public IObservable<UnitData> OnAddCardEvent => _addUnit;
    public IObservable<UnitData> OnRemoveCardEvent => _removeUnit;
    public IObservable<RelicItemBase> OnAddRelicItemEvent => _addRelicItem;
    public IObservable<RelicItemBase> OnRemoveRelicItemEvent => _removeRelicItem;
    public IObservable<string> OnAddPassEventName => _addPassEventName;
    public IObservable<BattleReportStruct> OnUpdateBattleReport => _battleReport;
    public List<UnitData> CurrentDeck { get { return _currentDeck; } }
    public RelicItemBase[] CurrentRelic { get { return _currentRelicItem; } }
    public string[] CurrentPassEvent { get { return _currentPassEvent; } }
    public int CurrentMoney { get {return _playerMoney; } }
    public int CurrentPopularity { get { return _playerPopularity; } }
    public BattleReportStruct BattleReportStruct { get { return _battleReportStruct; } }


    public void ChangePopularity(int value)
    {
        _changePopularity.OnNext(value);
    }

    public void ChangeMoney(int value)
    {
        _changeMoney.OnNext(value);
    }

    public void AddCard(UnitData unit)
    {
        _addUnit.OnNext(unit);
    }

    public void RemoveCard(UnitData unit)
    {
        _removeUnit.OnNext(unit);
    }

    public void AddRelicItem(RelicItemBase relicItem)
    {
        Debug.Log(relicItem);
        _addRelicItem.OnNext(relicItem);
    }

    public void AddPassEventName(string name)
    {
        _addPassEventName.OnNext(name);
    }

    public void RemoveRelicItem(RelicItemBase relicItem )
    {
        _removeRelicItem.OnNext(relicItem);
    }


    public void SetCurrentRelic(RelicItemBase[] relicItems)
    {
        _currentRelicItem = relicItems;
    }

    public void SetCurrentMoney(int money)
    {
        _playerMoney = money;
    }

    public void SetCurrentDeck(List<UnitData> currentDeck)
    {
        _currentDeck = currentDeck;
    }

    public void SetCurrentPassEvent(string[] events)
    {
        _currentPassEvent = events;
    }

    public void SetCurrentPopularity(int popularity)
    {
        _playerPopularity = popularity;
    }
    public void SetUpdateBattleReport(BattleReportStruct battleReport)
    {
        _battleReportStruct = battleReport;
    }

    public void UpdateBattleReport(int normalBattle = 0, int eliteBattle = 0, int bossBattle = 0, int eventUnit = 0, int shopUnit = 0, int depth = 0)
    {
        BattleReportStruct updateBattleReport = new BattleReportStruct(normalBattle, eliteBattle, bossBattle, eventUnit, shopUnit, depth);
        _battleReport.OnNext(updateBattleReport);
    }

}
