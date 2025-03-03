using Scenes.MainScene.Player;
using UnityEngine;
using System.Linq;
using UniRx;
using System;

namespace Scenes.MainScene.Upgrade
{
    public class UnitUpgradeModel
    {
        private UnitUpgradeInfo[] _upgradeInfos;
        private Subject<UnitStatus[]> _upgradeChoices = new Subject<UnitStatus[]>();

        public IObservable<UnitStatus[]> OnUpdateChoices => _upgradeChoices;
        private string _baseUnitName;

        public void Init()
        {
            _upgradeInfos = Resources.Load<UnitUpgradeInfoPool>("Value/UnitUpgradePool").upgradeInfos;
        }

        public void BeginUpgradeUnit(String unitName)
        {
            var targetBaseUnit = _upgradeInfos.FirstOrDefault(u => u.baseUnit.status.name == unitName);
            if (targetBaseUnit == null || targetBaseUnit == default) return;
            _baseUnitName = unitName;
            _upgradeChoices.OnNext(targetBaseUnit.UpgradeUnit.Select(u => u.status).ToArray());
        }

        public void UpdateUnit(string unitName)
        {
            var playerDeck = PlayerSingleton.Instance.CurrentDeck;
            var baseUnit = Resources.Load<CardPool>("Value/PlayerAllUnitPool").cards.FirstOrDefault(c => c.status.name == _baseUnitName);
            var upgradedUnit = Resources.Load<CardPool>("Value/PlayerAllUnitPool").cards.FirstOrDefault(c => c.status.name == unitName);
            var baseUnitCount = playerDeck.Count(d => d == baseUnit);
            for (int i = 0; i < baseUnitCount; i++)
            {
                PlayerSingleton.Instance.RemoveCard(baseUnit);
                PlayerSingleton.Instance.AddCard(upgradedUnit);
            }
        }

        public UnitStatus[] UpgradableUnits()
        {
            var playerUnits = PlayerSingleton.Instance.CurrentDeck;
            playerUnits = playerUnits.Distinct().ToList();
            playerUnits = playerUnits.Intersect(_upgradeInfos.Select(u => u.baseUnit)).ToList();
            return playerUnits.Select(u => u.status).ToArray();
        }
    }
}