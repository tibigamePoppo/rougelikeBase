using System.Linq;
using Scenes.MainScene;
using UnityEngine;
using TMPro;

public class BattleUnitDetailView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _detailText;
    [SerializeField] Transform _detailPanel;
    private Vector3 OFFSET = new Vector3(1f, 0.5f, 0);

    public void Init()
    {
        _detailPanel.gameObject.SetActive(false);
    }

    public void Show(UnitView unit)
    {
        var listText = ClacEnemysList(unit.eventUnit.unitType, unit.eventUnit.depth, unit.eventUnit.seed);
        var rewordlistText = ClacBattleRewordList(unit.eventUnit.unitType, unit.eventUnit.depth, unit.eventUnit.seed);
        _detailText.text = listText;
        _detailText.text += "\n" + rewordlistText;

        _detailPanel.gameObject.SetActive(true);
        _detailPanel.position = unit.imageTransform.transform.position + OFFSET;
    }

    public void Hide()
    {
        _detailPanel.gameObject.SetActive(false);
        _detailText.text = "";
    }

    private string ClacEnemysList(UnitType type,int stageDepth, int seed)
    {
        var enemyLevel = EnemyLevel.Normal;
        if(type == UnitType.Elite)
        {
            enemyLevel = EnemyLevel.Elite;
        }
        else if (type == UnitType.Boss)
        {
            enemyLevel = EnemyLevel.Boss;
        }
        EnemyDataPool dataPool = Resources.Load<EnemyDataPool>("Value/EnemyPool");
        UnitEnemyGroup[] unitEnemyData = new UnitEnemyGroup[6];
        UnityEngine.Random.InitState(seed);
        switch (enemyLevel)
        {
            case EnemyLevel.Normal:
                var depthFilterNormal = dataPool.normalPool.Where(pool => pool.minStageDepth <= stageDepth && pool.maxStageDepth >= stageDepth).Select(pool => pool.unitGroupData).ToArray();
                unitEnemyData = depthFilterNormal[UnityEngine.Random.Range(0, depthFilterNormal.Length)];
                break;
            case EnemyLevel.Elite:
                var depthFilterElite = dataPool.elitePool.Where(pool => pool.minStageDepth <= stageDepth && pool.maxStageDepth >= stageDepth).Select(pool => pool.unitGroupData).ToArray();
                unitEnemyData = depthFilterElite[UnityEngine.Random.Range(0, depthFilterElite.Length)];
                break;
            case EnemyLevel.Boss:
                var depthFilterBoss = dataPool.bossPool.Where(pool => pool.minStageDepth <= stageDepth && pool.maxStageDepth >= stageDepth).Select(pool => pool.unitGroupData).ToArray();
                unitEnemyData = depthFilterBoss[UnityEngine.Random.Range(0, depthFilterBoss.Length)];
                break;
            default:
                break;
        }
        var mergeunitEnemyData = unitEnemyData.SelectMany(e => e.unitData).ToArray();
        var characterCount = mergeunitEnemyData
            .GroupBy(c => c.status.name)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .ToList();
        var text = "";
        foreach (var entry in characterCount)
        {
            text += $"{entry.Name} : {entry.Count} \n";
        }
        return text;
    }

    private string ClacBattleRewordList(UnitType type, int stageDepth, int seed)
    {
        var text = "";
        var enemyLevel = EnemyLevel.Normal;
        if (type == UnitType.Elite)
        {
            enemyLevel = EnemyLevel.Elite;
        }
        else if (type == UnitType.Boss)
        {
            enemyLevel = EnemyLevel.Boss;
        }
        var _cards = Resources.Load<CardPool>("Value/PlayerAllUnitPool").cards.ToArray();
        Random.InitState(seed);
        switch (enemyLevel)
        {
            case EnemyLevel.Normal:
                _cards = _cards.Where(c => c.shopCost <= 110).ToArray();
                break;
            case EnemyLevel.Elite:
                _cards = _cards.Where(c => c.shopCost <= 170).ToArray();
                break;
            case EnemyLevel.Boss:
                _cards = _cards.Where(c => c.shopCost <= 250).ToArray();
                break;
            default:
                break;
        }
        var rewardUnit = _cards[Random.Range(0, _cards.Length)];
        int _money = 0;
        Random.InitState(seed);
        switch (enemyLevel)
        {
            case EnemyLevel.Normal:
                _money = Random.Range(90, 110);
                break;
            case EnemyLevel.Elite:
                _money = Random.Range(140, 170);
                break;
            case EnemyLevel.Boss:
                _money = Random.Range(210, 250);
                break;
            default:
                break;
        }
        text += $"カード報酬:{rewardUnit.status.name}\n";
        text += $"お金報酬　:{_money} G";
        return text;
    }
}
