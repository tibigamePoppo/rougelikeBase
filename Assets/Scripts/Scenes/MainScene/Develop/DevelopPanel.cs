using System.Collections;
using System.Collections.Generic;
using Scenes.MainScene.Relic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class DevelopPanel : MonoBehaviour
{
    RelicItemBase[] _relicItem;
    [SerializeField] private Button _button;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Transform _panelTransform;
    [SerializeField] private Transform _panelButtonTransform;
    [SerializeField] private GameObject _upgradeWindow;
    void Start()
    {
        Init();
    }

    public void Init()
    {
        _closeButton.OnClickAsObservable().Subscribe(_ => gameObject.SetActive(false)).AddTo(this);
        _openButton.OnClickAsObservable().Subscribe(_ => gameObject.SetActive(true)).AddTo(this);
        _upgradeButton.OnClickAsObservable().Subscribe(_ => _upgradeWindow.SetActive(true)).AddTo(this);
        gameObject.SetActive(false);
        _relicItem = Resources.Load<RelicItemPool>("Value/RelicItemPool").relicItem;
        foreach (var item in _relicItem)
        {
            var button = Instantiate(_button, _panelButtonTransform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = item._relicItemName;
            var relic = Instantiate(item, _panelTransform);
            button.OnClickAsObservable().First().Subscribe(_ => relic.Init()).AddTo(this);
        }
    }
}
