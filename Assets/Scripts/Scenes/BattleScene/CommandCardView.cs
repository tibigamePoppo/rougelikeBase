using Scenes.Battle.UnitCharacter;
using Scenes.MainScene.Cards;
using Scenes.MainScene.Player;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Scenes.Battle
{
    public class CommandCardView : MonoBehaviour
    {
        [SerializeField] private CardView _cardView;
        [SerializeField] private Image _commandColor;
        private string _setUnitName;
        public string SetUnitName { get { return _setUnitName; } }

        public void Init(UnitStatus unit,CharacterUnitModel characterUnitModel)
        {
            _setUnitName = unit.name;
            _cardView.Init(unit);
            _commandColor.color = Color.clear;
            characterUnitModel.OnChangeCommand.Subscribe(c => SetColor(c)).AddTo(this);
        }

        public void SetColor(MoveCommand moveCommand)
        {
            switch (moveCommand)
            {
                case MoveCommand.Charge :
                    _commandColor.color = Color.blue;
                    break;
                case MoveCommand.Lark:
                    _commandColor.color = Color.yellow;
                    break;
                case MoveCommand.Back:
                    _commandColor.color = Color.red;
                    break;
                default:
                    _commandColor.color = Color.clear;
                    break;
            }
        }
    }
}
