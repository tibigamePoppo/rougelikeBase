using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using Scenes.MainScene.Player;

public class RewardView : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private Transform _rewardItemPanel;
    [SerializeField] private RewardItemView _rewardItem;
    [SerializeField] private RewordSelectCardView _rewordSelectCard;

    [SerializeField] private Sprite _moneySprite;
    [SerializeField] private Sprite _unitSprite;


    private RewardCard _rewardCard;


    public void Init(EnemyLevel enemyLevel, int seed)
    {
        _backButton.OnClickAsObservable().Subscribe(_ => BackScene()).AddTo(this);

        RewardItemActionBase rewardMoney = new RewardMoney();
        rewardMoney.Init(enemyLevel, seed, null);
        Reward testReward = new Reward(rewardMoney.ContentName, _moneySprite, rewardMoney);
        var reward = Instantiate(_rewardItem, _rewardItemPanel);
        reward.Init(testReward);

        _rewardCard = new RewardCard();
        _rewardCard.Init(enemyLevel, seed, ShowCardSelectPanel);
        Reward testReward2 = new Reward(_rewardCard.ContentName, _unitSprite, _rewardCard);
        var reward2 = Instantiate(_rewardItem, _rewardItemPanel);
        reward2.Init(testReward2);
    }

    private void ShowCardSelectPanel()
    {
        var units = _rewardCard.ReawrdUnit;
        _rewordSelectCard.Init(units);
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
