using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using Scenes.MainScene.Player;
using Scenes.MainScene.Cards;

namespace Scenes.MainScene.Decks
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Transform _content;
        [SerializeField] private CardPanelView _cardPanel;
        private List<CardPanelView> instanceCardList = new List<CardPanelView>();

        public void Init(List<UnitData> playerUnit)
        {
            _backButton.OnClickAsObservable().Subscribe(_ => gameObject.SetActive(false)).AddTo(this);

            var uniqueUnits = playerUnit
                .GroupBy(c => c.name)
                .ToDictionary(g => g.Key, g => new { Data = g.First(), Count = g.Count() });

            foreach (var unit in uniqueUnits)
            {
                var unitData = unit.Value.Data;
                int unitCount = unit.Value.Count;

                var characterUnit = Instantiate(_cardPanel, _content);
                characterUnit.Init(unitData, unitCount);
                instanceCardList.Add(characterUnit);
            }
            gameObject.SetActive(false);
        }

        public void ActiveWindow(List<UnitData> playerUnit)
        {
            gameObject.SetActive(true);

            var uniqueUnits = playerUnit
                .GroupBy(c => c.status.name)
                .ToDictionary(g => g.Key, g => new { Data = g.First(), Count = g.Count() });

            foreach (var unit in uniqueUnits)
            {
                var existingCard = instanceCardList.FirstOrDefault(c => c.Card.name == unit.Key);
                if (existingCard != null)
                {
                    existingCard.UpdateView(unit.Value.Count);
                }
                else
                {
                    var unitData = unit.Value.Data;
                    int unitCount = unit.Value.Count;

                    var characterUnit = Instantiate(_cardPanel, _content);
                    characterUnit.Init(unitData, unitCount);
                    instanceCardList.Add(characterUnit);
                }
            }
        }
    }
}