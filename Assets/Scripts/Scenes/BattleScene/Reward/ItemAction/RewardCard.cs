using Scenes.MainScene.Player;
using UnityEngine;
using System.Linq;

public class RewardCard : RewardItemActionBase
{
    private const int CARD_COUNT = 3;
    private UnitData[] _rewardUnit = new UnitData[CARD_COUNT];
    public UnitData[] ReawrdUnit { get { return _rewardUnit; } }
    private System.Action _callback;
    public override void ItemAction()
    {
        if (PlayerSingleton.Instance)
        {
            _callback.Invoke();
        }
        else
        {
            Debug.LogWarning("Current scene not exist PlayerSingleton. cannot emit  get card reward");
        }
    }

    public override void Init(EnemyLevel enemyLevel,int seed, System.Action callback)
    {
        _callback = callback;
        var _cards = Resources.Load<CardPool>("Value/PlayerAllUnitPool").cards.ToArray();
        Random.InitState(seed);
        switch (enemyLevel)
        {
            case EnemyLevel.Normal:
                _cards = _cards.Where(c => c.shopCost <= 110).ToArray();
                break;
            case EnemyLevel.Elite:
                _cards = _cards.Where(c => c.shopCost <= 170).ToArray();
                break;
            case EnemyLevel.Boss:
                _cards = _cards.Where(c => c.shopCost <= 250).ToArray();
                break;
            default:
                break;
        }
        _rewardUnit[0] = _cards[Random.Range(0, _cards.Length)];
        _rewardUnit[1] = _cards[Random.Range(0, _cards.Length)];
        _rewardUnit[2] = _cards[Random.Range(0, _cards.Length)];
    }

    public override string ContentName { get { return "ユニットカード"; } }
}

