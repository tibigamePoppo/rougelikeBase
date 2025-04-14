using UnityEngine;

public abstract class  RewardItemActionBase : MonoBehaviour
{
    public virtual void ItemAction() { }
    public virtual void Init(EnemyLevel enemyLevel,int seed, System.Action callback) { }
    public virtual string ContentName { get; }
}
