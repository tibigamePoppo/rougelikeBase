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
        [SerializeField] private GameObject _stageClearText;
        [SerializeField] private GameObject _stageDefeatText;

        private List<UnitView> _instanceEventUnitList = new List<UnitView>();
        private Subject<EventUnit> _eventForword = new Subject<EventUnit>();
        public IObservable<EventUnit> OnEventForword => _eventForword ;

        public void Init(List<EventUnit>[] unitInfo)
        {
            _stageClearText.SetActive(false);
            _stageDefeatText.SetActive(false);
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
                        _eventForword.OnNext(d);
                    }).AddTo(this);

                    unit.gameObject.name = $"unit_{i}_{j}";

                    if (i != 0)
                    {
                        unit.Intaractable(false);
                    }

                    // to Battle end boss
                    if(unitInfo[i][j].unitType == UnitType.Boss)
                    {
                        unit.IsPlayerWinBattle.Subscribe(isWin =>
                        {
                            if (isWin)
                            {
                                _stageClearText.SetActive(true);
                            }
                            else
                            {
                                _stageDefeatText.SetActive(true);

                            }
                        }).AddTo(this);
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


        public void UnitUpdate(EventUnit[] EventUnits)
        {
            if (EventUnits == null) return;
            var pastEventUnit = _instanceEventUnitList.Where(e => e.eventUnit.depth <= EventUnits.First().depth).ToArray();
            foreach (var units in pastEventUnit)
            {
                units.Intaractable(false);
                units.FadeUnit(true);
            }
            var nextEventUnit = _instanceEventUnitList.Where(e => EventUnits.Contains(e.eventUnit)).ToArray();
            foreach (var units in nextEventUnit)
            {
                units.Intaractable(true);
                units.FadeUnit(false);
            }
        }
    }
}