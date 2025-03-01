using System.Collections;
using System.Collections.Generic;
using Scenes.Battle.UnitCharacter;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;

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

        public void Init(CharacterUnitModel[] units)
        {
            _meleeUnits = units.Where(u => u.WeaponType == MainScene.Player.UnitWeaponType.Melee).Select(u => u.Transform).ToArray();
            _rangeUnits = units.Where(u => u.WeaponType == MainScene.Player.UnitWeaponType.Range).Select(u => u.Transform).ToArray();
            _formationO.OnClickAsObservable().Subscribe(_ => Formation(FormationType.FormationO)).AddTo(this);
            _formationI.OnClickAsObservable().Subscribe(_ => Formation(FormationType.FormationI)).AddTo(this);
            _formationV.OnClickAsObservable().Subscribe(_ => Formation(FormationType.FormationV)).AddTo(this);
            _formationA.OnClickAsObservable().Subscribe(_ => Formation(FormationType.FormationA)).AddTo(this);
        }

        private void Formation(FormationType type)
        {
            Vector3[] path;
            var clac = new BattleFormationClac();
            switch (type)
            {
                case FormationType.FormationO:
                    path = new Vector3[] {new Vector3(10, 0, 0), new Vector3(8.2f, 0, 8.2f), new Vector3(0, 0, 10),
                                          new Vector3(-8.2f, 0, 8.2f), new Vector3(-10, 0, 0), new Vector3(-8.2f, 0, -8.2f),
                                          new Vector3(0, 0, -10), new Vector3(8.2f, 0, -8.2f) };
                    clac.FormationO(_meleeUnits, _rangeUnits, path);
                    break;
                case FormationType.FormationI:
                    path = new Vector3[] { new Vector3(-10, 0, 1), new Vector3(10, 0, 1) };
                    clac.FormationI(_meleeUnits, _rangeUnits, path);
                    break;
                case FormationType.FormationV:
                    path = new Vector3[] { new Vector3(-10, 0, 1), new Vector3(0, 0, -5), new Vector3(10, 0, 1) };
                    clac.FormationI(_meleeUnits, _rangeUnits, path);
                    break;
                case FormationType.FormationA:
                    path = new Vector3[] { new Vector3(-10, 0, 1), new Vector3(0, 0, 7), new Vector3(10, 0, 1) };
                    clac.FormationI(_meleeUnits, _rangeUnits, path);
                    break;
                default:
                    break;
            }
        }
    }
}