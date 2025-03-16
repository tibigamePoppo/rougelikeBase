using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using System;

public class EventSceneButtonView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _popularityText;
    [SerializeField] private GameObject _battle;
    [SerializeField] private GameObject _shop;
    private Button _button;
    private Subject<Unit> _click = new Subject<Unit>();
    public IObservable<Unit> OnClick => _click;


    public void Init(EventEffectArg arg)
    {
        _battle.SetActive(false);
        _shop.SetActive(false);
        _text.text = arg.text;
        SetMoney(arg.playerMoneyChange);
        SetPopularity(arg.playerPopularityChange);
        SetUnitScene(arg.changeScene);

        _button = GetComponent<Button>();
        _button.OnClickAsObservable().Subscribe(_ => _click.OnNext(default)).AddTo(this);
    }

    public void Interactable(bool eble)
    {
        _button.interactable = eble;
    }

    private void SetUnitScene(SceneName sceneName)
    {
        switch (sceneName)
        {
            case SceneName.BattleScene:
                _battle.SetActive(true);
                break;
            case SceneName.ShopScene:
                _shop.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void SetMoney(int money)
    {
        if(money != 0)
        {
            _moneyText.text = money >= 0 ? $"<color=#38b48b>  ▲{money} </color>" : $" <color=#cd5e3c> ▼{-money} </color>";
        }
        else
        {
            _moneyText.transform.parent.gameObject.SetActive(false);
        }
    }

    private void SetPopularity(int popularity)
    {
        if (popularity != 0)
        {
            _popularityText.text = popularity >= 0 ? $"<color=#38b48b> ▲{popularity} </color>" : $" <color=#cd5e3c> ▼{-popularity} </color>";
        }
        else
        {
            _popularityText.transform.parent.gameObject.SetActive(false);
        }
    }


}
