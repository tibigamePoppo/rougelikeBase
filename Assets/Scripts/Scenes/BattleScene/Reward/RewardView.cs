using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

public class RewardView : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private Transform _rewardItemPanel;
    [SerializeField] private RewardItemView _rewardItem;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        _backButton.OnClickAsObservable().Subscribe(_ => BackScene()).AddTo(this);

        Reward testReward = new Reward("MONEY!!", null, new RewardMoney());
        var reward = Instantiate(_rewardItem, _rewardItemPanel);
        reward.Init(testReward);
        Reward testReward2 = new Reward("CARD!!", null, new RewardCard());
        var reward2 = Instantiate(_rewardItem, _rewardItemPanel);
        reward2.Init(testReward2);
    }

    private void BackScene()
    {
        SceneManager.UnloadSceneAsync("BattleScene");
    }
}
