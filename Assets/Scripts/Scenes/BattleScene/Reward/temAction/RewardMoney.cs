using UnityEngine;

public class RewardMoney : RewardItemActionBase
{
    public override void ItemAction()
    {
        if(PlayerSingleton.Instance)
        {
            PlayerSingleton.Instance.ChangeMoney(100);
        }
        else
        {
            Debug.LogWarning("Current scene not exist PlayerSingleton. cannot emit  get money reward");
        }
    }
}
