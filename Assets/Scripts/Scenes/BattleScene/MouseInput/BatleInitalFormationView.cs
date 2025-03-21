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

        private Transform[] _meleeUnits;
        private Transform[] _rangeUnits;

        private float unitsScale;

        public void Init(CharacterUnitModel[] units)
        {
            var meleeModels = units.Where(u => u.WeaponType == MainScene.Player.UnitWeaponType.Melee).OrderBy(u => u.UnitName).ToArray();
            _meleeUnits = OrderUnit(meleeModels).Select(u => u.Transform).ToArray();
            _rangeUnits = units.Where(u => u.WeaponType == MainScene.Player.UnitWeaponType.Range).OrderBy(u => u.UnitName).Select(u => u.Transform).ToArray();
            _formationO.OnClickAsObservable().Subscribe(_ => Formation(FormationType.FormationO)).AddTo(this);
            _formationI.OnClickAsObservable().Subscribe(_ => Formation(FormationType.FormationI)).AddTo(this);
            _formationV.OnClickAsObservable().Subscribe(_ => Formation(FormationType.FormationV)).AddTo(this);
            _formationA.OnClickAsObservable().Subscribe(_ => Formation(FormationType.FormationA)).AddTo(this);

            unitsScale = 1 + Math.Max(_meleeUnits.Length, _rangeUnits.Length) / 2;
        }

        private CharacterUnitModel[] OrderUnit(CharacterUnitModel[] units)
        {
            List<CharacterUnitModel> newList1 = new List<CharacterUnitModel>();
            List<CharacterUnitModel> newList2 = new List<CharacterUnitModel>();
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
    }
}