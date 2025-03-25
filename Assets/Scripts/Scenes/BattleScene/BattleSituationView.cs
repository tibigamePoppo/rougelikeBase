using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Scenes.Battle.UnitCharacter;
using UniRx;
using DG.Tweening;
using System.Linq;

namespace Scenes.Battle
{
    public class BattleSituationView : MonoBehaviour
    {
        [SerializeField] private Image _playerSituation;
        [SerializeField] private GameObject _barPosition;
        [SerializeField] private TextMeshProUGUI _numberText;
        private CharacterUnitModel[] _playerUnits;
        private CharacterUnitModel[] _enemyUnits;
        private int _playerUnitsCount;
        private int _enemyUnitsCount;
        private const int width = 450;

        public void Init(CharacterUnitModel[] playerUnits, CharacterUnitModel[] enemyUnits)
        {
            _playerUnitsCount = playerUnits.Length;
            _enemyUnitsCount = enemyUnits.Length;
            _playerUnits = playerUnits;
            _enemyUnits = enemyUnits;
            foreach (var playerUnit in playerUnits)
            {
                playerUnit.OnChangeStateType.Subscribe(_ => UpdateSituation()).AddTo(this);
            }
            foreach (var enemyUnit in enemyUnits)
            {
                enemyUnit.OnChangeStateType.Subscribe(_ => UpdateSituation()).AddTo(this);
            }
        }

        private void UpdateSituation()
        {
            float situationValue = Mathf.Min(1, (PlayerUnitAlive()/ _playerUnitsCount) / (EnemyUnitAlive() / _enemyUnitsCount + PlayerUnitAlive() / _playerUnitsCount));

            _numberText.text = $"{PlayerUnitAlive()} | {EnemyUnitAlive()}";
            _playerSituation.DOFillAmount(situationValue, 0.1f);
            _barPosition.transform.DOLocalMoveX(-450 + width * situationValue * 2, 0.1f);
        }

        private float PlayerUnitAlive()
        {
            return _playerUnits.Count(u => u.CurrentState != UnitCharacter.State.CharacterUnitStateType.Dead);
        }
        private float EnemyUnitAlive()
        {
            return _enemyUnits.Count(u => u.CurrentState != UnitCharacter.State.CharacterUnitStateType.Dead);
        }
    }
}