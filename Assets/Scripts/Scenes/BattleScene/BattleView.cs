using System.Collections;
using System.Collections.Generic;
using Scenes.Battle.Enemy;
using UnityEngine;

namespace Scenes.Battle
{
    public class BattleView : MonoBehaviour
    {
        [SerializeField] private EnemyView _enemyView;
        public void Init(EnemyLevel enemyLevel)
        {
            EnemyDataPool dataPool =  Resources.Load<EnemyDataPool>("Value/EnemyPool");
            EnemyData enemyData = new EnemyData();
            switch (enemyLevel)
            {
                case EnemyLevel.Normal:
                    enemyData = dataPool.normalPool[Random.Range(0, dataPool.normalPool.Count)];
                    break;
                case EnemyLevel.Elite:
                    enemyData = dataPool.elitePool[Random.Range(0, dataPool.normalPool.Count)];
                    break;
                case EnemyLevel.Boss:
                    enemyData = dataPool.bossPool[Random.Range(0, dataPool.normalPool.Count)];
                    break;
                default:
                    break;
            }

            var enemyObject = Instantiate(_enemyView,transform);
            enemyObject.Init(enemyData);
        }
    }
}