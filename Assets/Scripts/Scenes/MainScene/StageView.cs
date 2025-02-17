using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System.Linq;

namespace Scenes.MainScene
{
    public class StageView : MonoBehaviour
    {
        [SerializeField] private Transform _stagContent;
        [SerializeField] private UnitView _unit;
        [SerializeField] private CharacterIconView _iconView;
        [SerializeField] private GameObject _layerUnit;
        [SerializeField] private UnitLineView _unitLineView;
        private List<UnitView> _instanceEventUnitList = new List<UnitView>();
        private Subject<int> _depthForword = new Subject<int>();
        public IObservable<int> OnDepthForword => _depthForword;

        public void Init(List<EventUnit>[] unitInfo)
        {
            InstanceUnits(unitInfo);
            LinqUnitLine();
        }

        private void InstanceUnits(List<EventUnit>[] unitInfo)
        {
            for (int i = unitInfo.Length - 1; i >= 0; i--)
            {
                var layer = Instantiate(_layerUnit, _stagContent);
                for (int j = 0; j < unitInfo[i].Count; j++)
                {
                    var unit = Instantiate(_unit, layer.transform);
                    unit.Init(unitInfo[i][j], _iconView);
                    unit.OnClickEvent.Subscribe(d =>
                    {
                        _depthForword.OnNext(d);
                    }).AddTo(this);
                    unit.gameObject.name = $"unit_{i}_{j}";
                    if (i != 0)
                    {
                        unit.Intaractable(false);
                    }

                    _instanceEventUnitList.Add(unit);
                }
            }
        }

        private void LinqUnitLine()
        {
            foreach (var unit in _instanceEventUnitList)
            {
                if (unit.eventUnit.connect == null) continue;

                foreach (var connectUnit in unit.eventUnit.connect)
                {
                    var unitLine = Instantiate(_unitLineView, unit.transform);
                    unitLine.Init(unit.transform, _instanceEventUnitList.FirstOrDefault(d => connectUnit.Equals(d.eventUnit)).transform);
                }
            }
        }


        public void UnitUpdate(int currentDepth)
        {
            var pastEventUnit = _instanceEventUnitList.Where(e => e.eventUnit.depth <= currentDepth).ToArray();
            foreach (var units in pastEventUnit)
            {
                units.Intaractable(false);
            }
            var nextEventUnit = _instanceEventUnitList.Where(e => e.eventUnit.depth == currentDepth + 1).ToArray();
            foreach (var units in nextEventUnit)
            {
                units.Intaractable(true);
            }
        }
    }
}