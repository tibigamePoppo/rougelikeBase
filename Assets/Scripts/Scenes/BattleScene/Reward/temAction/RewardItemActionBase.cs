using UnityEngine;

public abstract class RewardItemActionBase : MonoBehaviour
{
    public virtual void ItemAction() { }
    public virtual void Init(EnemyLevel enemyLevel,int seed) { }
    public virtual string ContentName { get; }
}
