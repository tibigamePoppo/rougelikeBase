using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

public class RewardView : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private Transform _rewardItemPanel;
    [SerializeField] private RewardItemView _rewardItem;

    [SerializeField] private Sprite _moneySprite;
    [SerializeField] private Sprite _unitSprite;

    public void Init()
    {
        _backButton.OnClickAsObservable().Subscribe(_ => BackScene()).AddTo(this);

        RewardItemActionBase rewardMoney = new RewardMoney();
        rewardMoney.Init();
        Reward testReward = new Reward(rewardMoney.ContentName, _moneySprite, rewardMoney);
        var reward = Instantiate(_rewardItem, _rewardItemPanel);
        reward.Init(testReward);

        RewardItemActionBase rewardCard = new RewardCard();
        rewardCard.Init();
        Reward testReward2 = new Reward(rewardCard.ContentName, _unitSprite, rewardCard);
        var reward2 = Instantiate(_rewardItem, _rewardItemPanel);
        reward2.Init(testReward2);
    }

    private void BackScene()
    {
        SceneManager.UnloadSceneAsync("BattleScene");
    }
}
