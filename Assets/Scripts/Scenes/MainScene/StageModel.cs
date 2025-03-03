using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum UnitType
{
    Battle = 0,
    Event = 1,
    Shop = 2,
    Elite = 3,
    Boss = 4,
    Start = 5,
}

public struct EventUnit
{
    public EventUnit[] connect;
    public UnitType unitType;
    public int depth;
    public EventUnit(EventUnit[] connect, UnitType unitType,int depth)
    {
        this.connect = connect;
        this.unitType = unitType;
        this.depth = depth;
    }
}

namespace Scenes.MainScene
{
    public class StageModel
    {
        private int _currentDepth = 0;
        private int _stageDepth = 8;
        private List<EventUnit>[] _unitInfos;
        public List<EventUnit>[] UnitInfo { get { return _unitInfos; } }
        public int CurrentDepth { get { return _currentDepth; } }

        private int[] _unitCount = new int[] { 2, 3, 4 };
        private float[] _unitCountWeight = new float[] { 0.5f, 1f, 0.8f };
        private int[] _unitCnnectValue = new int[] { 1, 2, 3 };
        private float[] _unitCnnectWeight = new float[] { 1f, 1.2f, 0.3f };
        private int[] _unitEvent = new int[] { 0, 1, 2, 3 };
        private float[] _unitEventWeight = new float[] { 1.5f, 1f, 0.3f, 0.5f };

        public void Init()
        {
            StageGenerate();
        }

        private void StageGenerate()
        {
            _unitInfos = new List<EventUnit>[_stageDepth];
            for (int i = _stageDepth - 1; i >= 0; i--)
            {
                _unitInfos[i] = new List<EventUnit>();
                int layerInUnitCount = UnitCountByDepth(i);
                for (int j = 0; j < layerInUnitCount; j++)
                {
                    var newUnit = new EventUnit(null, GetRandomUnitType(i), i);
                    if (i != _stageDepth - 1)
                    {
                        int connectSize = WeightRandom.RandomInt(_unitCnnectValue, _unitCnnectWeight);
                        int connectSkip = j;
                        if (j + connectSize > _unitInfos[i + 1].Count)
                        {
                            connectSkip = _unitInfos[i + 1].Count - connectSize;
                        }

                        if(j == layerInUnitCount - 1 && j + connectSize < _unitInfos[i + 1].Count)
                        {
                            connectSize = _unitInfos[i + 1].Count - j;
                        }

                        newUnit.connect = _unitInfos[i + 1].Skip(connectSkip).Take(connectSize).ToArray();
                    }
                    _unitInfos[i].Add(newUnit);
                }
                if(i != _stageDepth - 1)
                {
                    if (_unitInfos[i + 1].Count <= 0) continue;
                    for (int j = 0; j < _unitInfos[i + 1].Count; j++)
                    {
                        if (!_unitInfos[i].Any(u => u.connect.Contains(_unitInfos[i + 1][j])))
                        {
                            _unitInfos[i + 1].Remove(_unitInfos[i + 1][j]);
                        }
                    }
                }
            }
        }

        private int UnitCountByDepth(int depth)
        {
            return depth == _stageDepth - 1 ? 1 : WeightRandom.RandomInt(_unitCount, _unitCountWeight);
        }

        private UnitType GetRandomUnitType(int depth)
        {
            if (depth == _stageDepth - 1)
            {
                return UnitType.Boss;
            }
            else if (depth == 0)
            {
                return UnitType.Start;
            }
            else
            {
                return (UnitType)WeightRandom.RandomInt(_unitEvent, _unitEventWeight);
            }
        }

        public EventUnit[] NextDepth(EventUnit eventUnit)
        {
            _currentDepth = eventUnit.depth;
            return eventUnit.connect;
        }
    }
}