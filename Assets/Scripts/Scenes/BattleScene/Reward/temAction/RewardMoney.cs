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

    public override void Init()
    {
        _money = 100;
    }

    public override string ContentName { get { return _money.ToString(); } }
}
