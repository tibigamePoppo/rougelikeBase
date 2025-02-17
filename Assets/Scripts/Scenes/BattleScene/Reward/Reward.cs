using UnityEngine;

public struct Reward
{
    public string name;
    public Sprite sprite;
    public RewardItemActionBase action;
    public Reward(string name,Sprite sprite, RewardItemActionBase action)
    {
        this.name = name;
        this.sprite = sprite;
        this.action = action;
    }
}
