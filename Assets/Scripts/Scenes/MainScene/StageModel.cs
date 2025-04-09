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

public class EventUnit
{
    public EventUnit[] connect;
    public UnitType unitType;
    public int layerNumber;
    public int depth;
    public int stageNumber;
    public int seed;
    public EventUnit(EventUnit[] connect, UnitType unitType,int depth, int layerNumber,int stageNumber, int seed)
    {
        this.connect = connect;
        this.unitType = unitType;
        this.depth = depth;
        this.layerNumber = layerNumber;
        this.stageNumber = stageNumber;
        this.seed = seed;
    }
}

namespace Scenes.MainScene
{
    public class StageModel
    {
        private int _currentDepth = 0;
        private int[] _stageDepth = { 11, 13, 15 };
        private int _currentStage = 0; //0 to 2
        private const int MAXSTAGECOUNT = 3;
        private List<EventUnit>[] _unitInfos;
        public List<EventUnit>[] UnitInfo { get { return _unitInfos; } }
        public int CurrentDepth { get { return _currentDepth; } }

        private int _unitCount = 5;

        public void Init()
        {
            _currentStage = 0;// after big loop change 0 to 2
            StageGenerate();
        }

        public void NextStage()
        {
            _currentStage = _currentStage < (MAXSTAGECOUNT - 1) ? _currentStage + 1: _currentStage;
            StageGenerate();
        }

        private void StageGenerate()
        {
            _unitInfos = new List<EventUnit>[_stageDepth[_currentStage]];
            for (int i = 0; i < _unitInfos.Length; i++)
            {
                _unitInfos[i] = new List<EventUnit>();
                int layerInUnitCount = UnitCountByDepth(i);
                for (int j = 0; j < layerInUnitCount; j++)
                {
                    var newUnit = new EventUnit(new EventUnit[0], GetRandomUnitType(i), i, j, _currentStage, Random.Range(0, 100000));
                    _unitInfos[i].Add(newUnit);
                }
            }
            for (int i = 0; i < 6; i++)
            {
                int randomStartIndex = Random.Range(0, 3);
                var connectEventUnit = _unitInfos[0][randomStartIndex];
                while (true)
                {
                    connectEventUnit = SetConnetUnit(connectEventUnit);
                    if (connectEventUnit == null) break;
                }
            }
            RemoveNoneConnectUnit();
            FixEventUnits();
        }

        private EventUnit SetConnetUnit(EventUnit baseUnit)
        {
            if (baseUnit.unitType == UnitType.Boss) return null;// boss
            else if (baseUnit.depth == _stageDepth[_currentStage] - 2) // befor boss
            {
                var boss = UnitInfo[_stageDepth[_currentStage] - 1].First();
                baseUnit.connect = baseUnit.connect.Concat(new EventUnit[] { boss }).ToArray();
                return boss; 
            }
            else
            {
                int number = baseUnit.unitType != UnitType.Start ? baseUnit.layerNumber : baseUnit.layerNumber * 2 + 1;
                int depth = baseUnit.depth;
                var connetCandidate = _unitInfos[depth + 1].Skip(number - 1).Take(3).ToArray();
                int randomCount = Random.Range(0, connetCandidate.Length);
                baseUnit.connect = baseUnit.connect.Concat(new EventUnit[] { connetCandidate[randomCount] }).ToArray();
                return connetCandidate[randomCount];
            }
        }

        private void RemoveNoneConnectUnit()
        {
            foreach (var units in UnitInfo)
            {
                for (int i = units.Count - 1; i >= 0; i--)
                {
                    if (units[i].unitType == UnitType.Boss) continue;
                    else if (units[i].connect.Length == 0) units.RemoveAt(i);
                    else units[i].connect = units[i].connect.Distinct().ToArray();
                }
            }
        }

        private void FixEventUnits()
        {
            const int MAXCOUNT = 20;
            int outCount = 0;
            while (true)
            {
                bool isChange = false;
                outCount++;

                for (int i = 1; i < UnitInfo.Length - 1; i++) // without start and boss
                {
                    var units = UnitInfo[i];
                    if(units.Count > 4)
                    {
                        var singleConnectUnit = units.Where(u => u.connect.Length == 1).First();
                        var singleConnectUnit2 = units.Where(u => u.connect.Length == 1).Skip(1).First();
                        var exConnectUnit = UnitInfo[i - 1].Where(u => u.connect.Contains(singleConnectUnit)).ToArray();
                        foreach (var exUnit in exConnectUnit)
                        {
                            exUnit.connect = exUnit.connect.Where(c => c != singleConnectUnit).Concat(new EventUnit[] { singleConnectUnit2 }).ToArray();
                        }
                        singleConnectUnit2.connect = singleConnectUnit2.connect.Concat(singleConnectUnit.connect).Distinct().ToArray();
                        units.Remove(singleConnectUnit);
                    }
                    for (int j = units.Count - 1; j >= 0; j--)
                    {
                        var exConnectUnit = UnitInfo[i - 1].Where(u => u.connect.Contains(units[j])).ToArray();
                        var connectUnits = units[j].connect;

                        if (units.All(u => u.unitType == UnitType.Elite))
                        {
                            units[Random.Range(0, units.Count)].unitType = units[j].unitType = GetRandomUnitType(0, _unitEventWeight_nonelite);
                            isChange = true;
                        }
                        if (units[j].unitType == UnitType.Shop && connectUnits.Any(u => u.unitType == UnitType.Shop))
                        {
                            units[j].unitType = GetRandomUnitType(0, _unitEventWeight_nonshop);
                            isChange = true;
                        }

                        if (exConnectUnit.Any(u => u.unitType == UnitType.Event) &&
                           units[j].unitType == UnitType.Event &&
                           connectUnits.Any(u => u.unitType == UnitType.Event))
                        {
                            if (connectUnits.Count(u => u.unitType == UnitType.Event) >= 2)
                            {
                                units[j].unitType = GetRandomUnitType(0, _unitEventWeight_nonevent);
                            }
                            else
                            {
                                connectUnits.First().unitType = GetRandomUnitType(0, _unitEventWeight_nonevent);
                            }
                            isChange = true;

                        }

                        if (exConnectUnit.Any(u => u.unitType == UnitType.Battle) &&
                           units[j].unitType == UnitType.Battle &&
                           connectUnits.Any(u => u.unitType == UnitType.Battle))
                        {
                            if (connectUnits.Count(u => u.unitType == UnitType.Battle) >= 2)
                            {
                                units[j].unitType = GetRandomUnitType(0, _unitEventWeight_nonbattle);
                            }
                            else
                            {
                                connectUnits.First().unitType = GetRandomUnitType(0, _unitEventWeight_nonbattle);
                            }
                            isChange = true;

                        }
                    }
                }
                if (!isChange || outCount >= MAXCOUNT) break;
            }
        }

        private int UnitCountByDepth(int depth)
        {
            if (depth == 0) return 3;
            else if (depth == _stageDepth[_currentStage] - 1) return 1;
            else return _unitCount;
        }

        private int[] _unitEvent = new int[] {0, 1, 2, 3 };// battle, event, shop, elite
        private float[] _unitEventWeight = new float[] { 4f, 4f,1f,1f};
        private float[] _unitEventWeight_nonelite = new float[] { 4f, 4f, 2f, 0f };// depth 0 - 4  weght
        private float[] _unitEventWeight_nonevent = new float[] { 6f, 0f, 2f, 2f };
        private float[] _unitEventWeight_nonbattle = new float[] { 0, 6f, 2f, 2f };
        private float[] _unitEventWeight_nonshop = new float[] { 4f, 4f, 0f, 2f };// depth (boss - 3) - boss  weght
        private UnitType GetRandomUnitType(int depth,float[] weight = null)
        {
            if(weight != null)
            {
                return (UnitType)WeightRandom.RandomInt(_unitEvent, weight);
            }
            else if (depth == _stageDepth[_currentStage] - 1)
            {
                return UnitType.Boss;
            }
            else if (depth == 0)
            {
                return UnitType.Start;
            }
            else if (depth == 1)
            {
                return UnitType.Battle;
            }
            else if (depth == _stageDepth[_currentStage] - 2)
            {
                return UnitType.Shop;
            }
            else if (depth < 4)
            {
                return (UnitType)WeightRandom.RandomInt(_unitEvent, _unitEventWeight_nonelite);
            }
            else if (depth > _stageDepth[_currentStage] - 4)
            {
                return (UnitType)WeightRandom.RandomInt(_unitEvent, _unitEventWeight_nonshop);
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