using Scenes.MainScene.Player;
using UnityEngine;

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

    public override void Init()
    {
        var _cards = Resources.Load<CardPool>("Value/PlayerDeck").cards.ToArray();
        _rewardUnit = _cards[Random.Range(0, _cards.Length)];
    }

    public override string ContentName { get { return _rewardUnit.status.name; } }
}

