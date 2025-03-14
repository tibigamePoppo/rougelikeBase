using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShopItemView : MonoBehaviour
{
    [SerializeField] private Image _soldImage;
    [SerializeField] private TextMeshProUGUI _costText;
    protected int _shopCost;
    private bool _isBought = false;
    private Subject<Unit> _boughtEvent = new Subject<Unit>();
    protected GameObject _copyObject;
    public GameObject copyObject { get { return _copyObject; } }
    public IObservable<Unit> OnBoughtEvent => _boughtEvent;
    public int shopCost { get { return _shopCost; } }

    protected void BaseInit()
    {
        _soldImage.gameObject.SetActive(false);
        _costText.text = $"{_shopCost} G";
        Button button = GetComponent<Button>();
        if (PlayerSingleton.Instance.CurrentMoney >= _shopCost)
        {
            button.OnClickAsObservable().Where(_ => !_isBought && PlayerSingleton.Instance.CurrentMoney >= _shopCost).Subscribe(_ =>
            {
                Bought();
            }).AddTo(this);
        }
        else
        {
            UpdateText();
        }
    }

    public virtual void Bought()
    {
        _isBought = true;
        _soldImage.gameObject.SetActive(true);
        PlayerSingleton.Instance.ChangeMoney(-_shopCost);
        _boughtEvent.OnNext(default);
    }

    public void UpdateText()
    {
        if (PlayerSingleton.Instance.CurrentMoney < _shopCost)
        {
            _costText.color = Color.red;
            Button button = GetComponent<Button>();
            button.interactable = false;
        }
    }
}
