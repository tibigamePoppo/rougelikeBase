using System.Collections;
using System.Collections.Generic;
using Scenes.Battle.UnitCharacter;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using System;

namespace Scenes.Battle
{
    public class BatleInitalFormationView : MonoBehaviour
    {
        [SerializeField] private Button _formationO;
        [SerializeField] private Button _formationI;
        [SerializeField] private Button _formationV;
        [SerializeField] private Button _formationA;
        [SerializeField] private UnitGroupView _unitGroupPrefab;
        [SerializeField] private Transform _unitGroupParent;
        private List<UnitGroupView> _unitGroupViews = new List<UnitGroupView>();

        private Transform[] _meleeUnits;
        private Transform[] _rangeUnits;

        private float unitsScale;

        public void Init(CharacterUnitModel[] units)
        {
            var meleeModels = units.Where(u => u.WeaponType == MainScene.Player.UnitWeaponType.Melee).OrderBy(u => u.UnitName).ToArray();
            var _orderedMeleeUnits = OrderUnit(meleeModels).Select(u => u.Transform).ToArray();
            var riderUnits = units.Where(u => u.WeaponType == MainScene.Player.UnitWeaponType.Rider).OrderBy(u => u.UnitName).Select(u => u.Transform).ToArray();
            _meleeUnits = _orderedMeleeUnits.Concat(riderUnits).ToArray();
            _rangeUnits = units.Where(u => u.WeaponType == MainScene.Player.UnitWeaponType.Range).OrderBy(u => u.UnitName).Select(u => u.Transform).ToArray();
            _formationO.OnClickAsObservable().Subscribe(_ =>
            {
                Formation(FormationType.FormationO);
                UpdateUnitGroupViewPosition();
                Formation(FormationType.FormationO);
            }).AddTo(this);
            _formationI.OnClickAsObservable().Subscribe(_ =>
            {
                Formation(FormationType.FormationI);
                UpdateUnitGroupViewPosition();
                Formation(FormationType.FormationI);
            }).AddTo(this);
            _formationV.OnClickAsObservable().Subscribe(_ =>
            {
                Formation(FormationType.FormationV);
                UpdateUnitGroupViewPosition();
                Formation(FormationType.FormationV);
            }).AddTo(this);
            _formationA.OnClickAsObservable().Subscribe(_ =>
            {
                Formation(FormationType.FormationA);
                UpdateUnitGroupViewPosition();
                Formation(FormationType.FormationA);
            }).AddTo(this);

            unitsScale = 1 + Math.Max(_meleeUnits.Length, _rangeUnits.Length) / 2;

            if (riderUnits != null && riderUnits.Length > 0)
            {
                GroupUnitInstance(riderUnits);
            }
            GroupUnitInstance(_orderedMeleeUnits);
            GroupUnitInstance(_rangeUnits);
        }

        private void GroupUnitInstance(Transform[] units)
        {
            int riderGroupCount = Math.Max((units.Length - 1) / 10 + 1, 1);
            for (int i = 0; i < riderGroupCount; i++)
            {
                var groupSprite = Instantiate(_unitGroupPrefab, _unitGroupParent);
                int unitCount = i < (riderGroupCount - 1) ? 10 : (units.Length - i * 10) % 10;
                unitCount = unitCount == 0 ? 10 : unitCount;
                var child = units.Skip(i * 10).Take(unitCount).ToArray();
                groupSprite.Init(child);
                _unitGroupViews.Add(groupSprite);
            }
        }

        private void UpdateUnitGroupViewPosition()
        {
            foreach (var groupSprite in _unitGroupViews)
            {
                groupSprite.UpdatePosition();
            }
        }

        public void OnGameStart()
        {
            foreach (var groupSprite in _unitGroupViews)
            {
                groupSprite.OnGameStart();
            }
            gameObject.SetActive(false);
        }

        private CharacterUnitModel[] OrderUnit(CharacterUnitModel[] units)
        {
            var newList1 = new List<CharacterUnitModel>();
            var newList2 = new List<CharacterUnitModel>();
            int count = 0;
            while (true)
            {
                newList1.Add(units[count]);
                count++;
                if (count >= units.Length) break;
                newList2.Add(units[count]);
                count++;
                if (count >= units.Length) break;
            }
            newList2.Reverse();
            return newList1.Concat(newList2).ToArray();
        }

        private void Formation(FormationType type)
        {
            Vector3[] path;
            var clac = new BattleFormationClac();
            switch (type)
            {
                case FormationType.FormationO:
                    int points = (int)unitsScale / 2 > 18 ? (int)unitsScale / 2 : 18;
                    path = new Vector3[points];
                    for (int i = 0; i < points; i++)
                    {
                        float angle = i * (360f / (points - 1));
                        path[i] = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
                    }
                    float radius = unitsScale / 2 > 3f ? unitsScale / 2 : 3;
                    path = path.Select(v => v * radius).ToArray();
                    clac.FormationO(_meleeUnits, _rangeUnits, path);
                    break;
                case FormationType.FormationI:
                    path = new Vector3[] { new Vector3(-1f, 0, 0.1f), new Vector3(1, 0, 0.1f) };
                    path = path.Select(v => v * unitsScale).ToArray();
                    clac.FormationI(_meleeUnits, _rangeUnits, path);
                    break;
                case FormationType.FormationV:
                    path = new Vector3[] { new Vector3(-1f, 0, 0.1f), new Vector3(0, 0, -0.5f), new Vector3(1, 0, 0.1f) };
                    path = path.Select(v => v * unitsScale).ToArray();
                    clac.FormationI(_meleeUnits, _rangeUnits, path);
                    break;
                case FormationType.FormationA:
                    path = new Vector3[] { new Vector3(-1f, 0, 0.1f), new Vector3(0, 0, 0.7f), new Vector3(1, 0, 0.1f) };
                    path = path.Select(v => v * unitsScale).ToArray();
                    clac.FormationI(_meleeUnits, _rangeUnits, path);
                    break;
                default:
                    break;
            }
        }

        public void EnemyInitialFormation(int type, CharacterUnitModel[] units)
        {
            var _enemyMeleeUnits = units.Where(u => u.WeaponType == MainScene.Player.UnitWeaponType.Melee).OrderBy(u => u.UnitName).Select(u => u.Transform).ToArray();
            var _enemyRangeUnits = units.Where(u => u.WeaponType == MainScene.Player.UnitWeaponType.Range).OrderBy(u => u.UnitName).Select(u => u.Transform).ToArray();

            var enemyUnitsScale = 1 + Math.Max(_enemyMeleeUnits.Length, _enemyRangeUnits.Length) / 2;
            Vector3[] path;
            var clac = new BattleFormationClac();
            Vector3 offsetVector = new Vector3(0, 0, 30);
            switch (type)
            {
                case 0:
                    int points = (int)unitsScale / 2 > 18 ? (int)unitsScale / 2 : 18;
                    path = new Vector3[points];
                    for (int i = 0; i < points; i++)
                    {
                        float angle = 360 - i * (360f / (points - 1));
                        path[i] = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
                    }
                    float radius = enemyUnitsScale + 10;
                    path = path.Select(v => v * radius).ToArray();
                    clac.FormationO(_enemyMeleeUnits, _enemyRangeUnits, path);
                    break;
                case 1:
                    path = new Vector3[] { new Vector3(1, 0, 0.1f), new Vector3(-1f, 0, 0.1f) };
                    path = path.Select(v => v * enemyUnitsScale + offsetVector).ToArray();
                    clac.FormationI(_enemyMeleeUnits, _enemyRangeUnits, path);
                    break;
                case 2:
                    path = new Vector3[] { new Vector3(1f, 0, 0.1f), new Vector3(0, 0, 0.5f), new Vector3(-1f, 0, 0.1f) };
                    path = path.Select(v => v * enemyUnitsScale + offsetVector).ToArray();
                    clac.FormationI(_enemyMeleeUnits, _enemyRangeUnits, path);
                    break;
                case 3:
                    path = new Vector3[] { new Vector3(1f, 0, 0.1f), new Vector3(0, 0, -0.7f), new Vector3(-1f, 0, 0.1f) };
                    path = path.Select(v => v * enemyUnitsScale + offsetVector).ToArray();
                    clac.FormationI(_enemyMeleeUnits, _enemyRangeUnits, path);
                    break;
                default:
                    break;
            }
        }
    }
}