using UnityEngine;
using TMPro;

namespace Scenes.MainScene
{
    public class StageEndScoreView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI normalEnemy;
        [SerializeField] TextMeshProUGUI eliteEnemy;
        [SerializeField] TextMeshProUGUI bossEnemy;
        [SerializeField] TextMeshProUGUI shopUnit;
        [SerializeField] TextMeshProUGUI eventUnit;
        [SerializeField] TextMeshProUGUI depth;
        public void Init()
        {
            var battleReport = PlayerSingleton.Instance.BattleReportStruct;
            normalEnemy.text = battleReport.normalBattle.ToString();
            eliteEnemy.text = battleReport.eliteBattle.ToString();
            bossEnemy.text = battleReport.bossBattle.ToString();
            shopUnit.text = battleReport.shopUnit.ToString();
            eventUnit.text = battleReport.eventUnit.ToString();
            depth.text = battleReport.depth.ToString();
        }
    }
}