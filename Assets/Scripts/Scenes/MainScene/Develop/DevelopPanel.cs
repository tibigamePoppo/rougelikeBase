using System.Collections;
using System.Collections.Generic;
using Scenes.MainScene.Relic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine.SceneManagement;
using Scenes.Battle;
using Scenes.EventScene;

public class DevelopPanel : MonoBehaviour
{
    RelicItemBase[] _relicItem;
    [SerializeField] private Button _button;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _getMoneyButton;
    [SerializeField] private Button _getPopularityButton;
    [SerializeField] private Button _battleButton;
    [SerializeField] private Button _battleEliteButton;
    [SerializeField] private Button _battleBossButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _eventButton;
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
        _getMoneyButton.OnClickAsObservable().Subscribe(_ => PlayerSingleton.Instance.ChangeMoney(100));
        _getPopularityButton.OnClickAsObservable().Subscribe(_ => PlayerSingleton.Instance.ChangePopularity(100));
        _battleButton.OnClickAsObservable().Subscribe(_ => SceneLoad(UnitType.Battle)).AddTo(this);
        _battleEliteButton.OnClickAsObservable().Subscribe(_ => SceneLoad(UnitType.Elite)).AddTo(this);
        _battleBossButton.OnClickAsObservable().Subscribe(_ => SceneLoad(UnitType.Boss)).AddTo(this);
        _shopButton.OnClickAsObservable().Subscribe(_ => SceneLoad(UnitType.Shop)).AddTo(this);
        _eventButton.OnClickAsObservable().Subscribe(_ => SceneLoad(UnitType.Event)).AddTo(this);
        gameObject.SetActive(false);
        _relicItem = Resources.Load<RelicItemPool>("Value/RelicItemPool").relicItem;
        foreach (var item in _relicItem)
        {
            var button = Instantiate(_button, _panelButtonTransform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = item.relicItemName;
            var relic = Instantiate(item, _panelTransform);
            button.OnClickAsObservable().First().Subscribe(_ => relic.Init()).AddTo(this);
        }
    }

    private EnemyLevel _enemyLevel;
    private void SceneLoad(UnitType scene)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        switch (scene)
        {
            case UnitType.Battle:
                SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                _enemyLevel = EnemyLevel.Normal;
                break;
            case UnitType.Boss:
                SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                _enemyLevel = EnemyLevel.Boss;
                break;
            case UnitType.Elite:
                SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
                _enemyLevel = EnemyLevel.Elite;
                break;
            case UnitType.Shop:
                SceneManager.LoadScene("ShopScene", LoadSceneMode.Additive);
                break;
            case UnitType.Event:
                SceneManager.LoadScene("EventScene", LoadSceneMode.Additive);
                break;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeSceneSingleton.Instance.FadeOutEffet();
        if (scene.name == "BattleScene")
        {
            BattlePresenter battlePresenter = FindFirstObjectByType<BattlePresenter>();
            if (battlePresenter != null)
            {
                battlePresenter.Init(_enemyLevel, PlayerSingleton.Instance.CurrentDeck, 0);
            }
        }
        else if (scene.name == "EventScene")
        {
            EventScenePtesenter eventPresenter = FindFirstObjectByType<EventScenePtesenter>();
            eventPresenter.Init(0);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
