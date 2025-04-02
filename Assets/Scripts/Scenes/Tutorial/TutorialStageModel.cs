using System.Collections.Generic;
using System.Linq;

namespace Scenes.Tutorial
{
    public class TutorialStageModel
    {
        private int _currentDepth = 0;
        private int _stageDepth = 6;
        private List<EventUnit>[] _unitInfos;
        public List<EventUnit>[] UnitInfo { get { return _unitInfos; } }
        public int CurrentDepth { get { return _currentDepth; } }

        public void Init()
        {
            StageGenerate();
        }

        private void StageGenerate()
        {
            _unitInfos = new List<EventUnit>[_stageDepth];
            int connectSize = 1;
            for (int i = _stageDepth - 1; i >= 0; i--)
            {
                _unitInfos[i] = new List<EventUnit>();
                int layerInUnitCount = UnitCountByDepth(i);
                for (int j = 0; j < layerInUnitCount; j++)
                {
                    var newUnit = new EventUnit(null, GetRandomUnitType(i), i,j);
                    if (i != _stageDepth - 1)
                    {
                        int connectSkip = j;
                        if (j + connectSize > _unitInfos[i + 1].Count)
                        {
                            connectSkip = _unitInfos[i + 1].Count - connectSize;
                        }

                        if (j == layerInUnitCount - 1 && j + connectSize < _unitInfos[i + 1].Count)
                        {
                            connectSize = _unitInfos[i + 1].Count - j;
                        }

                        newUnit.connect = _unitInfos[i + 1].Skip(connectSkip).Take(connectSize).ToArray();
                    }
                    _unitInfos[i].Add(newUnit);
                }
                if (i != _stageDepth - 1)
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

        //<---Change Tutorial const value->
        private int UnitCountByDepth(int depth)
        {
            return 1;
        }

        //<---Change Tutorial const value->
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
            else if (depth == 1)
            {
                return UnitType.Event;
            }
            else if(depth == 2)
            {
                return UnitType.Battle;
            }
            else if (depth == 3)
            {
                return UnitType.Shop;
            }
            else if (depth == 4)
            {
                return UnitType.Elite;
            }
            else
            {
                return UnitType.Event;
            }
        }

        public EventUnit[] NextDepth(EventUnit eventUnit)
        {
            _currentDepth = eventUnit.depth;
            return eventUnit.connect;
        }
    }
}