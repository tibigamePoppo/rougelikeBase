using Scenes.MainScene.Relic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine.SceneManagement;
using Scenes.Battle;
using Scenes.EventScene;
using System.Linq;
using Scenes.MainScene.Upgrade;

public class DevelopPanel : MonoBehaviour
{
    RelicItemBase[] _relicItem;
    [SerializeField] private Button _button;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _get100MoneyButton;
    [SerializeField] private Button _get1000MoneyButton;
    [SerializeField] private Button _get100PopularityButton;
    [SerializeField] private Button _get1000PopularityButton;
    [SerializeField] private Button _battleButton;
    [SerializeField] private Button _battleEliteButton;
    [SerializeField] private Button _battleBossButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _eventButton;
    [SerializeField] private Transform _panelTransform;
    [SerializeField] private Transform _panelButtonTransform;
    [SerializeField] private UnitUpgradeView _upgradeWindow;
    void Start()
    {
        Init();
    }

    public void Init()
    {
        _closeButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => gameObject.SetActive(false)).AddTo(this);
        _openButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => gameObject.SetActive(true)).AddTo(this);
        _upgradeButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => _upgradeWindow.OpenPanel()).AddTo(this);
        _get100MoneyButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => PlayerSingleton.Instance.ChangeMoney(100));
        _get100PopularityButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => PlayerSingleton.Instance.ChangePopularity(100));
        _get1000MoneyButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => PlayerSingleton.Instance.ChangeMoney(1000));
        _get1000PopularityButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => PlayerSingleton.Instance.ChangePopularity(1000));
        _battleButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => SceneLoad(UnitType.Battle)).AddTo(this);
        _battleEliteButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => SceneLoad(UnitType.Elite)).AddTo(this);
        _battleBossButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => SceneLoad(UnitType.Boss)).AddTo(this);
        _shopButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => SceneLoad(UnitType.Shop)).AddTo(this);
        _eventButton.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => SceneLoad(UnitType.Event)).AddTo(this);
        gameObject.SetActive(false);
        _relicItem = Resources.Load<RelicItemPool>("Value/RelicItemPool").relicItem;
        foreach (var item in _relicItem)
        {
            var button = Instantiate(_button, _panelButtonTransform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"[R] {item.relicItemName}";
            var relic = Instantiate(item, _panelTransform);
            button.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).First().Subscribe(_ => relic.Init()).AddTo(this);
        }
        var _cardDataList = Resources.Load<CardPool>("Value/ShopUnitPool").CardList();
        foreach (var unit in _cardDataList)
        {
            var button = Instantiate(_button, _panelButtonTransform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"[U] {unit.status.name}";
            button.OnClickAsObservable().Where(_ => !IsActiveOtherScene()).Subscribe(_ => PlayerSingleton.Instance.AddCard(unit)).AddTo(this);
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

    private bool IsActiveOtherScene()
    {
        return Enumerable.Range(0, SceneManager.sceneCount)
                         .Select(SceneManager.GetSceneAt)
                         .Any(scene => scene.name == "BattleScene" || scene.name == "ShopScene" || scene.name == "EventScene");
    }
}
