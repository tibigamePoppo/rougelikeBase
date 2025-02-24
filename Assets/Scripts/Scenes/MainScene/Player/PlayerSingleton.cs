using Common;
using UniRx;
using System;
using Scenes.MainScene.Player;
using System.Collections.Generic;

public class PlayerSingleton : SingletonAbstract<PlayerSingleton>
{
    private Subject<int> _changeHp = new Subject<int>();
    private Subject<int> _changeMoney = new Subject<int>();
    private Subject<UnitData> _addUnit = new Subject<UnitData>();
    private List<UnitData> _currentDeck = new List<UnitData>();
    private int _playerMoney;

    public IObservable<int> OnChangeHpEvent => _changeHp;
    public IObservable<int> OnChangeMoneyEvent => _changeMoney;
    public IObservable<UnitData> OnAddCardEvent => _addUnit;
    public List<UnitData> CurrentDeck { get { return _currentDeck; } }
    public int CurrentMoney { get {return _playerMoney; } }

    public void ChangeHp(int value)
    {
        _changeHp.OnNext(value);
    }

    public void ChangeMoney(int value)
    {
        _changeMoney.OnNext(value);
    }

    public void AddCard(UnitData unit)
    {
        _addUnit.OnNext(unit);
    }

    public void SetCurrentMoney(int money)
    {
        _playerMoney = money;
    }

    public void SetCurrentDeck(List<UnitData> currentDeck)
    {
        _currentDeck = currentDeck;
    }
}
