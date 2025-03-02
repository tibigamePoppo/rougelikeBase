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
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Scenes.Battle
{
    public enum FormationType
    {
        None,
        FormationO,
        FormationI,
        FormationV,
        FormationA
    }

    public class BattleView : MonoBehaviour
    {
        [SerializeField] private Transform _playerUnitSpawnTransfrom;
        [SerializeField] private Transform[] _enemyUnitSpawnTransfrom;
        [SerializeField] private RewardView _rewardView;
        [SerializeField] private BattleFormationPresenter _battleFormationPresenter;
        [SerializeField] private BatleInitalFormationView _battleInitialFormationView;
        [SerializeField] private Button _battlReadyButton;
        private FormationType _formationType = FormationType.None;

        public void Init(EnemyLevel enemyLevel, List<UnitData> playerCards)
        {
            _rewardView.Init();
            _rewardView.gameObject.SetActive(false);

            EnemyDataPool dataPool =  Resources.Load<EnemyDataPool>("Value/EnemyPool");
            UnitData[] enemyData = new UnitData[0];
            switch (enemyLevel)
            {
                case EnemyLevel.Normal:
                    enemyData = dataPool.normalPool[UnityEngine.Random.Range(0, dataPool.normalPool.Count)]._unitData;
                    break;
                case EnemyLevel.Elite:
                    enemyData = dataPool.elitePool[UnityEngine.Random.Range(0, dataPool.elitePool.Count)]._unitData;
                    break;
                case EnemyLevel.Boss:
                    enemyData = dataPool.bossPool[UnityEngine.Random.Range(0, dataPool.bossPool.Count)]._unitData;
                    break;
                default:
                    break;
            }

            var playerPresenter = PlayerUnitSpawn(playerCards);
            var enemyPresenter = EnemyUnitSpawn(enemyData);
            var playerModel = playerPresenter.Select(p => p.CharacterUnitModel).ToArray();
            var enemyModel = enemyPresenter.Select(p => p.CharacterUnitModel).ToArray();

            _battleInitialFormationView.Init(playerModel);
            _battleFormationPresenter.Init(playerModel);

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

            InitalFormation(playerPresenter.Concat(enemyPresenter).ToList()).Forget();
        }

        public async UniTaskVoid InitalFormation(List<CharacterUnitPresenter> characters)
        {
            await UniTask.WaitUntil(() => _formationType == FormationType.None);
            _battlReadyButton.OnClickAsObservable().Subscribe(_ =>
            {
                foreach (var item in characters)
                {
                    item.BattleStart();
                }
                _battleInitialFormationView.gameObject.SetActive(false);
            });
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

        private List<CharacterUnitPresenter> EnemyUnitSpawn(UnitData[] enemyData)
        {
            List<CharacterUnitPresenter> enemyList = new List<CharacterUnitPresenter>();
            foreach (var enemy in enemyData)
            {
                Vector3 baseSpawnPosition = _enemyUnitSpawnTransfrom[UnityEngine.Random.Range(0, _enemyUnitSpawnTransfrom.Length)].position;
                Vector3 random = new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5)).normalized * 10;
                var instanceUnit = Instantiate(enemy.prefab, baseSpawnPosition + random, Quaternion.identity, _playerUnitSpawnTransfrom);
                enemyList.Add(instanceUnit.Init(enemy.status));
            }
            return enemyList;
        }

    }
}