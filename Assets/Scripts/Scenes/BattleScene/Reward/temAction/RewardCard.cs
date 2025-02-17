using UnityEngine;

public class RewardCard : RewardItemActionBase
{
    public override void ItemAction()
    {
        if (PlayerSingleton.Instance)
        {
            var _cards = Resources.Load<CardPool>("Value/PlayerDeck").cards;
            PlayerSingleton.Instance.AddCard(_cards[Random.RandomRange(0, _cards.Count)]);
        }
        else
        {
            Debug.LogWarning("Current scene not exist PlayerSingleton. cannot emit  get card reward");
        }
    }
}

