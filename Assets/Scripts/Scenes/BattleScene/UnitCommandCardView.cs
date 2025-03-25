using System.Collections.Generic;
using Scenes.MainScene.Cards;
using Scenes.MainScene.Player;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using Scenes.Battle.UnitCharacter;
using UnityEngine.UI;

namespace Scenes.Battle
{
    public class UnitCommandCardView : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private CommandCardView _cardView;
        [SerializeField] private HorizontalLayoutGroup _cardTransform;
        [SerializeField] private GameObject _commandView;
        [SerializeField] private GameObject _chargeCommand;
        [SerializeField] private GameObject _backCommand;
        [SerializeField] private GameObject _larkCommand;

        private Dictionary<GameObject,string> _commandUnitDictionary = new Dictionary<GameObject, string>();
        private string _pastUnitName = null;
        private CharacterUnitModel[] _playerCharacters;
        private const float cardWidth = 300;
        float _layoutWidth;

        public void Init(List<UnitData> playerCards, CharacterUnitModel[] playerCharacters)
        {
            _layoutWidth = _cardTransform.GetComponent<RectTransform>().rect.width;
            _playerCharacters = playerCharacters;
            _commandView.SetActive(false);
             var cards = playerCards.Distinct().ToArray();
            InstanceUnitView(cards);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _commandView.SetActive(true);
            var dragUnit = _commandUnitDictionary.FirstOrDefault(c => c.Key == eventData.pointerEnter);
            if(dragUnit.Key != default && dragUnit.Key != null)
            {
                _pastUnitName = dragUnit.Value;
            }
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            var dragUnit = _commandUnitDictionary.FirstOrDefault(c => c.Key == eventData.pointerEnter);
            if (dragUnit.Key != default && dragUnit.Key != null)
            {
                var commandUnits = _playerCharacters.Where(u => u.UnitName == dragUnit.Value).ToArray();
                foreach (var commandUnit in commandUnits)
                {
                    commandUnit.IsSelect(true);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var dragUnit = _commandUnitDictionary.FirstOrDefault(c => c.Key == eventData.pointerEnter);
            if (dragUnit.Key != default && dragUnit.Key != null)
            {
                var commandUnits = _playerCharacters.Where(u => u.UnitName == dragUnit.Value).ToArray();
                foreach (var commandUnit in commandUnits)
                {
                    commandUnit.IsSelect(false);
                }
            }
        }

        private void UnitMoveCommand(MoveCommand command)
        {
            var commandUnits = _playerCharacters.Where(u => u.UnitName == _pastUnitName).ToArray();
            switch (command)
            {
                case MoveCommand.Charge:
                    foreach (var unit in commandUnits)
                    {
                        unit.Charge();
                    }
                    break;
                case MoveCommand.Back:
                    foreach (var unit in commandUnits)
                    {
                        unit.Back();
                    }
                    break;
                case MoveCommand.Lark:
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
                var unitObject = Instantiate(_cardView, _cardTransform.transform);
                unitObject.Init(item.status, _playerCharacters.FirstOrDefault(u => u.UnitName == item.status.name));
                _commandUnitDictionary.Add(unitObject.gameObject,item.status.name);
            }
            if(_commandUnitDictionary.Count > _layoutWidth / cardWidth)
            {
                FixLayoutWidth();
            }
        }

        private void FixLayoutWidth()
        {
            float diff = _commandUnitDictionary.Count * cardWidth - _layoutWidth;
            float overlap = diff / (_commandUnitDictionary.Count - 1);
            _cardTransform.spacing = -overlap;
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