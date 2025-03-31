using UnityEngine;

public class RewardMoney : RewardItemActionBase
{
    private int _money;
    public override void ItemAction()
    {
        if(PlayerSingleton.Instance)
        {
            PlayerSingleton.Instance.ChangeMoney(_money);
        }
        else
        {
            Debug.LogWarning("Current scene not exist PlayerSingleton. cannot emit  get money reward");
        }
    }

    public override void Init(EnemyLevel enemyLevel)
    {
        switch (enemyLevel)
        {
            case EnemyLevel.Normal:
                _money = Random.Range(90, 110);
                break;
            case EnemyLevel.Elite:
                _money = Random.Range(140, 170);
                break;
            case EnemyLevel.Boss:
                _money = Random.Range(210, 250);
                break;
            default:
                break;
        }
    }

    public override string ContentName { get { return _money.ToString(); } }
}
