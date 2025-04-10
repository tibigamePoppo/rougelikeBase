using Scenes.MainScene.Player;
using UnityEngine;
using System.Linq;

public class RewardCard : RewardItemActionBase
{
    private UnitData _rewardUnit;
    public UnitData ReawrdUnit { get { return _rewardUnit; } }
    public override void ItemAction()
    {
        if (PlayerSingleton.Instance)
        {
            PlayerSingleton.Instance.AddCard(_rewardUnit);
        }
        else
        {
            Debug.LogWarning("Current scene not exist PlayerSingleton. cannot emit  get card reward");
        }
    }

    public override void Init(EnemyLevel enemyLevel,int seed)
    {
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
        _rewardUnit = _cards[Random.Range(0, _cards.Length)];
    }

    public override string ContentName { get { return _rewardUnit.status.name; } }
}

