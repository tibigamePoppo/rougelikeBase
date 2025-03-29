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
    None = 6,
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
        private int[] _stageDepth = { 11, 13, 15 };
        private int _currentStage = 0; //0 to 2
        private List<EventUnit>[] _unitInfos;
        public List<EventUnit>[] UnitInfo { get { return _unitInfos; } }
        public int CurrentDepth { get { return _currentDepth; } }

        private int _unitCount = 3;
        private int[] _unitCnnectValue = new int[] { 1, 2 };
        private float[] _unitCnnectWeight = new float[] { 3f, 1f};
        private UnitType[] _unitEvent;

        private int[] _eventUnitCount = { 3, 4, 6 };
        private int[] _shopUnitCount = { 1, 2, 2 };
        private int[] _normalUnitCount = { 4, 4, 4 };
        private int[] _eliteUnitCount = { 1, 2, 3 };

        public void Init()
        {
            _currentStage = 0;// after big loop change 0 to 2
            StageGenerate();
        }

        private void StageGenerate()
        {
            _unitEvent = GetUnitEvent(_currentStage);
            _unitInfos = new List<EventUnit>[_stageDepth[_currentStage]];
            for (int i = _stageDepth[_currentStage] - 1; i >= 0; i--)
            {
                _unitInfos[i] = new List<EventUnit>();
                int layerInUnitCount = UnitCountByDepth(i);
                for (int j = 0; j < layerInUnitCount; j++)
                {
                    //var newUnit = new EventUnit(null, GetRandomUnitType(i), i); // past method
                    var newUnit = new EventUnit(null, _unitEvent[i], i);
                    if (i != _stageDepth[_currentStage] - 1)
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
                if(i != _stageDepth[_currentStage] - 1)
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
            return depth == _stageDepth[_currentStage] - 1 ? 1 : _unitCount;
        }

        private UnitType[] GetUnitEvent(int stage)
        {
            var unitEvent = new UnitType[_stageDepth[stage]];
            for (int i = 0; i < _stageDepth[stage]; i++)
            {
                unitEvent[i] = UnitType.None;
            }
            int normalBattleCount = _normalUnitCount[stage];
            int eventCount = _eventUnitCount[stage];
            int shopCount = _shopUnitCount[stage];
            int[] randomType = { 0, 1 };
            int[] randomType2 = { 0, 1, 2 };
            float[] randomWeight1 = { 4, 6 };
            float[] randomWeight2 = { 1, 1 };
            float[] randomWeight3 = { 1.5f, 1.5f, 7 };
            float[] randomWeight4 = { 2f, 5f, 3 };
            switch (stage)
            {
                case 0:
                    unitEvent[0] = UnitType.Start;
                    unitEvent[1] = UnitType.Battle;
                    unitEvent[2] = (UnitType)WeightRandom.RandomInt(randomType, randomWeight1);
                    unitEvent[3] = (UnitType)WeightRandom.RandomInt(randomType, randomWeight1);
                    unitEvent[4] = (UnitType)WeightRandom.RandomInt(randomType, randomWeight1);
                    unitEvent[5] = UnitType.Elite;
                    unitEvent[6] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? (eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle) : UnitType.Event;
                    unitEvent[7] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? (eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle) : UnitType.Event;
                    unitEvent[8] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? (eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle) : UnitType.Event;
                    unitEvent[9] = UnitType.Shop;
                    unitEvent[10] = UnitType.Boss;
                    break;
                case 1:
                    unitEvent[0] = UnitType.Start;
                    unitEvent[1] = UnitType.Battle;
                    unitEvent[2] = (UnitType)WeightRandom.RandomInt(randomType, randomWeight2);
                    unitEvent[3] = (UnitType)WeightRandom.RandomInt(randomType2, randomWeight3);
                    unitEvent[4] = UnitType.Elite;
                    unitEvent[5] = (UnitType)WeightRandom.RandomInt(randomType, randomWeight2);
                    unitEvent[6] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event;
                    unitEvent[7] = shopCount > _unitEvent.Count(c => c == UnitType.Shop) ? (normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event) : UnitType.Shop ;
                    unitEvent[8] = UnitType.Elite;
                    unitEvent[9] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event;
                    unitEvent[10] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event;
                    unitEvent[11] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event;
                    unitEvent[12] = UnitType.Shop;
                    unitEvent[13] = UnitType.Boss;
                    break;
                case 2:
                    unitEvent[0] = UnitType.Start;
                    unitEvent[1] = UnitType.Battle;
                    unitEvent[2] = (UnitType)WeightRandom.RandomInt(randomType, randomWeight1);
                    unitEvent[3] = (UnitType)WeightRandom.RandomInt(randomType, randomWeight4);
                    unitEvent[4] = UnitType.Elite;
                    unitEvent[5] = (UnitType)WeightRandom.RandomInt(randomType, randomWeight2);
                    unitEvent[6] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event;
                    unitEvent[7] = shopCount > _unitEvent.Count(c => c == UnitType.Shop) ? (normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event) : (UnitType)WeightRandom.RandomInt(randomType, randomWeight2);
                    unitEvent[8] = UnitType.Elite;
                    unitEvent[9] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event;
                    unitEvent[10] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event;
                    unitEvent[11] = shopCount > _unitEvent.Count(c => c == UnitType.Shop) ? (normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event) : UnitType.Shop;
                    unitEvent[12] = UnitType.Elite;
                    unitEvent[13] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event;
                    unitEvent[14] = normalBattleCount > unitEvent.Count(c => c == UnitType.Battle) ? eventCount > unitEvent.Count(c => c == UnitType.Event) ? (UnitType)WeightRandom.RandomInt(randomType, randomWeight2) : UnitType.Battle : UnitType.Event;
                    unitEvent[15] = UnitType.Shop;
                    unitEvent[16] = UnitType.Boss;
                    break;
                default:
                    break;
            }
            return unitEvent;
        }

        /*  // past method
        private int[] _unitEvent = new int[] {0, 1, 2,3 };
        private float[] _unitEventWeight = new float[] { 3f, 1f,1f,1f};
        private UnitType GetRandomUnitType(int depth)
        {
            if (depth == _stageDepth[_currentStage] - 1)
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
        */

        public EventUnit[] NextDepth(EventUnit eventUnit)
        {
            _currentDepth = eventUnit.depth;
            return eventUnit.connect;
        }
    }
}