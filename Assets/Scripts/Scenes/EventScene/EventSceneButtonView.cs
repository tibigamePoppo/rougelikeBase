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
    private Button _button;
    private Subject<Unit> _click = new Subject<Unit>();
    public IObservable<Unit> OnClick => _click;


    public void Init(EventEffectArg arg)
    {
        _text.text = arg.text;
        SetMoney(arg.playerMoneyChange);
        SetPopularity(arg.playerPopularityChange);

        _button = GetComponent<Button>();
        _button.OnClickAsObservable().Subscribe(_ => _click.OnNext(default)).AddTo(this);
    }

    public void Interactable(bool eble)
    {
        _button.interactable = eble;
    }

    private void SetMoney(int money)
    {
        if(money != 0)
        {
            _moneyText.text = money.ToString();
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
            _popularityText.text = popularity.ToString();
        }
        else
        {
            _popularityText.transform.parent.gameObject.SetActive(false);
        }
    }


}
