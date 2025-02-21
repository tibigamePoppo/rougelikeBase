using Common;
using UniRx;
using System;
using Scenes.MainScene.Player;

public class PlayerSingleton : SingletonAbstract<PlayerSingleton>
{
    private Subject<int> _changeHp = new Subject<int>();
    private Subject<int> _changeMoney = new Subject<int>();
    private Subject<CardData> _addCard = new Subject<CardData>();
    public IObservable<int> OnChangeHpEvent => _changeHp;
    public IObservable<int> OnChangeMoneyEvent => _changeMoney;
    public IObservable<CardData> OnAddCardEvent => _addCard;

    public void ChangeHp(int value)
    {
        _changeHp.OnNext(value);
    }

    public void ChangeMoney(int value)
    {
        _changeMoney.OnNext(value);
    }

    public void AddCard(CardData card)
    {
        _addCard.OnNext(card);
    }
}
