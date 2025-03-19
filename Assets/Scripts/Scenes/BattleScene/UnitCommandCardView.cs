using System.Collections.Generic;
using Scenes.MainScene.Cards;
using Scenes.MainScene.Player;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using Scenes.Battle.UnitCharacter;

namespace Scenes.Battle
{
    public class UnitCommandCardView : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private CardView _cardView;
        [SerializeField] private Transform _cardTransform;
        [SerializeField] private GameObject _commandView;
        [SerializeField] private GameObject _chargeCommand;
        [SerializeField] private GameObject _backCommand;
        [SerializeField] private GameObject _larkCommand;

        private Dictionary<GameObject,string> card = new Dictionary<GameObject, string>();
        private string _pastUnitName = null;
        private CharacterUnitModel[] _playerCharacters;

        public void Init(List<UnitData> playerCards, CharacterUnitModel[] playerCharacters)
        {
            _playerCharacters = playerCharacters;
            _commandView.SetActive(false);
             var cards = playerCards.Distinct().ToArray();
            InstanceUnitView(cards);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _commandView.SetActive(true);
            var dragUnit = card.FirstOrDefault(c => c.Key == eventData.pointerEnter);
            if(dragUnit.Key != default && dragUnit.Key != null)
            {
                _pastUnitName = dragUnit.Value;
            }

            Debug.Log($"dragUnit {dragUnit}");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_pastUnitName == null) return;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _commandView.SetActive(false);
            if (_pastUnitName == null) return;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (_pastUnitName == null) return;
            if (eventData.pointerEnter == _chargeCommand.gameObject)
            {
                UnitMoveCommand(MoveCommand.Charge);
            }
            else if (eventData.pointerEnter == _backCommand.gameObject)
            {
                UnitMoveCommand(MoveCommand.Back);
            }
            else if (eventData.pointerEnter == _larkCommand.gameObject)
            {
                UnitMoveCommand(MoveCommand.Lark);
            }
        }

        private void UnitMoveCommand(MoveCommand command)
        {
            var commandUnits = _playerCharacters.Where(u => u.UnitName == _pastUnitName).ToArray();
            switch (command)
            {
                case MoveCommand.Charge:
                    Debug.Log("Command Charge!");
                    foreach (var unit in commandUnits)
                    {
                        unit.Charge();
                    }
                    break;
                case MoveCommand.Back:
                    Debug.Log("Command Back!");
                    foreach (var unit in commandUnits)
                    {
                        unit.Back();
                    }
                    break;
                case MoveCommand.Lark:
                    Debug.Log("Command Lark!");
                    foreach (var unit in commandUnits)
                    {
                        unit.Lark();
                    }
                    break;
                default:
                    break;
            }
        }

        private void InstanceUnitView(UnitData[] units)
        {
            foreach (var item in units)
            {
                var unitObject = Instantiate(_cardView, _cardTransform);
                unitObject.Init(item.status);
                card.Add(unitObject.gameObject,item.status.name);
            }
        }
    }

    public enum MoveCommand
    {
        Charge,
        Back,
        Lark,
        Stop
    }
    
}