using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Scenes.MainScene
{
    public class StageView : MonoBehaviour
    {
        [SerializeField] private Transform _stagContent;
        [SerializeField] private UnitView _unit;
        [SerializeField] private CharacterIconView _iconView;
        [SerializeField] private StageStartView _stageStartView;
        [SerializeField] private StageEndView _stageEndView;
        [SerializeField] private GameObject _layerUnit;
        [SerializeField] private UnitLineView _unitLineView;

        private List<UnitView> _instanceEventUnitList = new List<UnitView>();
        private List<GameObject> _stageGameObjects = new List<GameObject>();
        private Subject<EventUnit> _eventForword = new Subject<EventUnit>();
        private Subject<Unit> _nextStage = new Subject<Unit>();
        public IObservable<EventUnit> OnEventForword => _eventForword;
        public IObservable<Unit> OnNextStage => _nextStage;
        private int _stageDepth;

        public void Init(List<EventUnit>[] unitInfo)
        {
            _stageDepth = 1;
            _stageStartView.Init();
            _stageEndView.Init();
            InstanceUnits(unitInfo);
            LinqUnitLine();
            SceneManager.LoadScene("FadeSceneEffect", LoadSceneMode.Additive);
        }

        private void InstanceUnits(List<EventUnit>[] unitInfo)
        {
            for (int i = unitInfo.Length - 1; i >= 0; i--)
            {
                var layer = Instantiate(_layerUnit, _stagContent);
                _stageGameObjects.Add(layer);
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
                            BossBattleEnd(isWin);
                        }).AddTo(this);
                    }

                    _instanceEventUnitList.Add(unit);
                }
            }
        }

        public void BossBattleEnd(bool isWin)
        {
            if (isWin && _stageDepth == 3)
            {
                _stageEndView.ActiveWindow(EndType.Win);
            }
            else if (isWin)
            {
                _nextStage.OnNext(default);
                _stageDepth++;
            }
            else
            {
                _stageEndView.ActiveWindow(EndType.Defeat);
            }
        }

        private void LinqUnitLine()
        {
            foreach (var unit in _instanceEventUnitList)
            {
                if (unit.eventUnit.connect == null) continue;

                foreach (var connectUnit in unit.eventUnit.connect)
                {
                    var unitLine = Instantiate(_unitLineView, unit.imageTransform);
                    unitLine.Init(unit.imageTransform, _instanceEventUnitList.FirstOrDefault(d => connectUnit.Equals(d.eventUnit)).imageTransform);
                }
            }
        }

        public void StageRebuild(List<EventUnit>[] unitInfo)
        {
            DestroyStage();
            InstanceUnits(unitInfo);
            LinqUnitLine();
        }

        private void DestroyStage()
        {
            _iconView.transform.parent = transform;
            _iconView.transform.position = new Vector3(10000,0,0);
            _instanceEventUnitList.Clear();
            for (int i = 0; i < _stageGameObjects.Count; i++)
            {
                Destroy(_stageGameObjects[i]);
            }
        }

        public void UnitUpdate(EventUnit[] EventUnits)
        {
            if (EventUnits == null) return;
            var nextLayerEventUnit = _instanceEventUnitList.Where(e => e.eventUnit.depth <= EventUnits.First().depth).ToArray();
            nextLayerEventUnit = nextLayerEventUnit.Where(e => EventUnits.Contains(e.eventUnit)).ToArray();
            /*
            foreach (var units in nextLayerEventUnit)
            {
                units.Intaractable(false);
                units.FadeUnit(true);
            }
            */
            foreach (var view in _instanceEventUnitList)
            {
                view.Intaractable(false);
                view.FadeUnit(true);
            }
            foreach (var view in nextLayerEventUnit)
            {
                ActiveEventUnit(view);
            }
        }

        private void ActiveEventUnit(UnitView eventUnit)
        {
            eventUnit.Intaractable(true);
            eventUnit.FadeUnit(false);
            if (eventUnit.eventUnit.connect.Length == 0) return;
            var connectCount = eventUnit.eventUnit.connect;
            var connectView = _instanceEventUnitList.Where(e => connectCount.Contains(e.eventUnit)).ToArray();
            foreach (var view in connectView)
            {
                ActiveEventUnit(view);
            }
        }

    }
}