using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ResourcePool/EnemyDataPool")]
public class EnemyDataPool : ScriptableObject
{
    public List<EnemyData> normalPool;
    public List<EnemyData> elitePool;
    public List<EnemyData> bossPool;
}
