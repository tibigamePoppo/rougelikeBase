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

    public void Init(EnemyLevel enemyLevel, int seed)
    {
        _backButton.OnClickAsObservable().Subscribe(_ => BackScene()).AddTo(this);

        RewardItemActionBase rewardMoney = new RewardMoney();
        rewardMoney.Init(enemyLevel, seed);
        Reward testReward = new Reward(rewardMoney.ContentName, _moneySprite, rewardMoney);
        var reward = Instantiate(_rewardItem, _rewardItemPanel);
        reward.Init(testReward);

        RewardItemActionBase rewardCard = new RewardCard();
        rewardCard.Init(enemyLevel, seed);
        Reward testReward2 = new Reward(rewardCard.ContentName, _unitSprite, rewardCard);
        var reward2 = Instantiate(_rewardItem, _rewardItemPanel);
        reward2.Init(testReward2);
    }

    public void ShowDialog(bool win)
    {
        if (!win) _rewardItemPanel.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void BackScene()
    {
        SceneManager.UnloadSceneAsync("BattleScene");
    }
}
