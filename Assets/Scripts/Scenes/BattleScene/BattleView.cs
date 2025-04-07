using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scenes.Battle.UnitCharacter;
using Scenes.MainScene.Player;
using UnityEngine;
using System;
using UniRx.Triggers;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        [SerializeField] private BattleSituationView _battleSituationView;
        [SerializeField] private OnBattleFormationView _onBattleFormationView;
        [SerializeField] private UnitCommandCardView _unitCommandCardView;
        [SerializeField] private Button _battlReadyButton;
        [SerializeField] private EventSystem eventSystem;
        private EventSystem _pastEventSystem;
        private Camera _pastMainCamera;
        private Subject<bool> _isPlayerWinBattle = new Subject<bool>();
        private FormationType _formationType = FormationType.None;

        public IObservable<bool> IsPlayerWinBattle => _isPlayerWinBattle;

        private int[] _enemySpawnPaturnValue = new int[] { 0, 1, 2, 3 };//0 is circle, 1 is I formation, 2 is V formatin,3 is A formation
        private float[] _enemySpawnPaturnWeight = new float[] { 1f, 3f, 2f, 2f };

        public void Init(EnemyLevel enemyLevel, List<UnitData> playerCards,int stageDepth, EnemyData[] preliminaryEnemyData = null)
        {
            _pastEventSystem = EventSystem.current;
            EventSystem.current = eventSystem;
            _pastMainCamera = Camera.main;
            _pastMainCamera.gameObject.SetActive(false);
            _rewardView.Init(enemyLevel);
            _unitCommandCardView.gameObject.SetActive(false);
            _rewardView.gameObject.SetActive(false);

            UnitEnemyGroup[] unitEnemyData = new UnitEnemyGroup[6];
            if (preliminaryEnemyData == null || preliminaryEnemyData.Length == 0)
            {
                EnemyDataPool dataPool = Resources.Load<EnemyDataPool>("Value/EnemyPool");
                switch (enemyLevel)
                {
                    case EnemyLevel.Normal:
                        var depthFilterNormal = dataPool.normalPool.Where(pool => pool.minStageDepth <= stageDepth && pool.maxStageDepth >= stageDepth).Select(pool => pool._unitGroupData).ToArray();
                        unitEnemyData = depthFilterNormal[UnityEngine.Random.Range(0, depthFilterNormal.Length)];
                        break;
                    case EnemyLevel.Elite:
                        var depthFilterElite = dataPool.elitePool.Where(pool => pool.minStageDepth <= stageDepth && pool.maxStageDepth >= stageDepth).Select(pool => pool._unitGroupData).ToArray();
                        unitEnemyData = depthFilterElite[UnityEngine.Random.Range(0, depthFilterElite.Length)];
                        break;
                    case EnemyLevel.Boss:
                        var depthFilterBoss = dataPool.bossPool.Where(pool => pool.minStageDepth <= stageDepth && pool.maxStageDepth >= stageDepth).Select(pool => pool._unitGroupData).ToArray();
                        unitEnemyData = depthFilterBoss[UnityEngine.Random.Range(0, depthFilterBoss.Length)];
                        break;
                    default:
                        break;
                }
            }
            else
            {
                unitEnemyData = preliminaryEnemyData[UnityEngine.Random.Range(0, preliminaryEnemyData.Length)]._unitGroupData;
            }
            var playerPresenter = PlayerUnitSpawn(playerCards);
            var enemyPresenter = EnemyUnitSpawn(unitEnemyData, _enemyUnitSpawnTransfrom[0]);
            var playerModel = playerPresenter.Select(p => p.CharacterUnitModel).ToArray();
            var enemyModel = enemyPresenter.Select(e => e.Select(p => p.CharacterUnitModel).ToArray()).ToArray();
            _unitCommandCardView.Init(playerCards, playerModel);

            _battleInitialFormationView.Init(playerModel);
            _onBattleFormationView.Init();

            var mergeEnemyModel = enemyModel.SelectMany(e => e).ToArray();
            var mergeEnemyPresenter = enemyPresenter.SelectMany(e => e).ToArray();

            foreach (var pp in playerPresenter)
            {
                pp.SetGroup(playerModel, mergeEnemyModel);
            }
            foreach (var ep in mergeEnemyPresenter)
            {
                ep.SetGroup(mergeEnemyModel, playerModel);
            }
            foreach (var model in mergeEnemyModel)
            {
                model.Charge();
            }

            this.UpdateAsObservable()
                .Select(_ => playerModel.All(p => p.CurrentState == UnitCharacter.State.CharacterUnitStateType.Dead)) // すべての survive が false か判定
                .DistinctUntilChanged()
                .Where(allDead => allDead)
                .Subscribe(_ => BattleEnd(false))
                .AddTo(this);


            this.UpdateAsObservable()
                .Select(_ => mergeEnemyModel.All(p => p.CurrentState == UnitCharacter.State.CharacterUnitStateType.Dead)) // すべての survive が false か判定
                .DistinctUntilChanged()
                .Where(allDead => allDead)
                .Subscribe(_ => BattleEnd(true))
                .AddTo(this);



            _onBattleFormationView.OnCharge.Subscribe(_ =>
            {
                foreach (var model in playerModel)
                {
                    model.Charge();
                }
            }).AddTo(this);

            int enemySpawnPaturn = WeightRandom.RandomInt(_enemySpawnPaturnValue, _enemySpawnPaturnWeight);
            _battleInitialFormationView.EnemyInitialFormation(enemySpawnPaturn, enemyModel, _enemyUnitSpawnTransfrom);
            InitalFormation(playerPresenter.Concat(mergeEnemyPresenter).ToList()).Forget();
            _battleSituationView.Init(playerModel, mergeEnemyModel);
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
                _unitCommandCardView.gameObject.SetActive(true);
                _battleFormationPresenter.isStarted = true;
                _onBattleFormationView.Active();
                _battleInitialFormationView.OnGameStart();
            });
        }

        private void BattleEnd(bool playerWin)
        {
            _isPlayerWinBattle.OnNext(playerWin);
            _rewardView.ShowDialog(playerWin);
            _pastMainCamera.gameObject.SetActive(true);
            EventSystem.current = _pastEventSystem;
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

        private List<CharacterUnitPresenter>[] EnemyUnitSpawn(UnitEnemyGroup[] enemyData,Transform parentTransform)
        {
            List<CharacterUnitPresenter>[] enemyDataList = new List<CharacterUnitPresenter>[6];
            for (int i = 0; i < 6; i++)
            {
                List<CharacterUnitPresenter> enemyList = new List<CharacterUnitPresenter>();
                var enemyDataGroup = enemyData[i]._unitData;
                foreach (var enemy in enemyDataGroup)
                {
                    Vector3 random = Vector3.zero;
                    var instanceUnit = Instantiate(enemy.prefab, random, Quaternion.identity, parentTransform);
                    enemyList.Add(instanceUnit.Init(enemy.status));
                }
                enemyDataList[i] = enemyList;
            }
            return enemyDataList;
        }

    }
}