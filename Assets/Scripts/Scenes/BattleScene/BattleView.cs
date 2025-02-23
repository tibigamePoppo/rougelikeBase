using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scenes.Battle.Enemy;
using Scenes.Battle.UnitCharacter;
using Scenes.MainScene.Player;
using UnityEngine;
using System;
using UniRx.Triggers;
using UniRx;

namespace Scenes.Battle
{
    public class BattleView : MonoBehaviour
    {
        [SerializeField] private Transform _playerUnitSpawnTransfrom;
        [SerializeField] private RewardView _rewardView;

        public void Init(EnemyLevel enemyLevel, List<UnitData> playerCards)
        {
            _rewardView.Init();
            _rewardView.gameObject.SetActive(false);

            EnemyDataPool dataPool =  Resources.Load<EnemyDataPool>("Value/EnemyPool");
            EnemyData enemyData = new EnemyData();
            switch (enemyLevel)
            {
                case EnemyLevel.Normal:
                    enemyData = dataPool.normalPool[UnityEngine.Random.Range(0, dataPool.normalPool.Count)];
                    break;
                case EnemyLevel.Elite:
                    enemyData = dataPool.elitePool[UnityEngine.Random.Range(0, dataPool.elitePool.Count)];
                    break;
                case EnemyLevel.Boss:
                    enemyData = dataPool.bossPool[UnityEngine.Random.Range(0, dataPool.bossPool.Count)];
                    break;
                default:
                    break;
            }

            var playerPresenter = PlayerUnitSpawn(playerCards);
            var enemyPresenter = EnemyUnitSpawn(enemyData);
            var playerModel = playerPresenter.Select(p => p.CharacterUnitModel).ToArray();
            var enemyModel = enemyPresenter.Select(p => p.CharacterUnitModel).ToArray();
            foreach (var pp in playerPresenter)
            {
                pp.SetGroup(playerModel, enemyModel);
            }
            foreach (var ep in enemyPresenter)
            {
                ep.SetGroup(enemyModel, playerModel);
            }
            this.UpdateAsObservable()
                .Select(_ => playerModel.All(p => p.CurrentState == UnitCharacter.State.CharacterUnitStateType.Dead)) // すべての survive が false か判定
                .DistinctUntilChanged()
                .Where(allDead => allDead)
                .Subscribe(_ => BattleEnd())
                .AddTo(this);


            this.UpdateAsObservable()
                .Select(_ => enemyModel.All(p => p.CurrentState == UnitCharacter.State.CharacterUnitStateType.Dead)) // すべての survive が false か判定
                .DistinctUntilChanged()
                .Where(allDead => allDead)
                .Subscribe(_ => BattleEnd())
                .AddTo(this);
        }

        private void BattleEnd()
        {
            _rewardView.gameObject.SetActive(true);
        }

        private List<CharacterUnitPresenter> PlayerUnitSpawn(List<UnitData> playerUnits)
        {
            List<CharacterUnitPresenter> playerList = new List<CharacterUnitPresenter>();
            foreach (var unit in playerUnits)
            {
                Vector3 random = new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5));
                var instanceUnit = Instantiate(unit.prefab, _playerUnitSpawnTransfrom.position + random, Quaternion.identity, _playerUnitSpawnTransfrom);
                playerList.Add(instanceUnit.Init(unit.status));
            }
            return playerList;
        }

        private List<CharacterUnitPresenter> EnemyUnitSpawn(EnemyData enemyData)
        {
            const int enemyCount = 10;
            List<CharacterUnitPresenter> enemyList = new List<CharacterUnitPresenter>();
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 random = new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5)).normalized * 10;
                var instanceUnit = Instantiate(enemyData._unitData.prefab, random, Quaternion.identity, _playerUnitSpawnTransfrom);
                enemyList.Add(instanceUnit.Init(enemyData._unitData.status));
            }
            return enemyList;
        }

    }
}