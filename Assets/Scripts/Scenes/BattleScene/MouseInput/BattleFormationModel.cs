using System.Collections;
using System.Collections.Generic;
using Scenes.Battle.UnitCharacter;
using UnityEngine;
using System.Linq;
using UniRx;
using System;
using Scenes.MainScene.Player;

public class BattleFormationModel 
{
    private CharacterUnitModel[] _playerUnits = new CharacterUnitModel[0];
    private CharacterUnitModel[] _selectUnits = new CharacterUnitModel[0];

    private Subject<FormationPositionInfo> _formationPosition = new Subject<FormationPositionInfo>();

    public IObservable<FormationPositionInfo> OnFormationChange => _formationPosition;

    public void Init(CharacterUnitModel[] playerUnits)
    {
        _playerUnits = playerUnits;
    }

    public void SetSelectLine(Vector3[] vectors)
    {
        var circleInside = DetectUnitsInsideShape(vectors);
        _selectUnits = circleInside.OrderBy(u => u.UnitName).ToArray();
        foreach (var unit in _selectUnits)
        {
            Debug.Log($"内側のunit: {unit.Transform.gameObject.name}");
        }
    }

    public void SetFormationLine(Vector3[] vectors)
    {
        circLineOnUnits(vectors);
    }

    private void circLineOnUnits(Vector3[] vectors)
    {
        if (_selectUnits.Length <= 0 || vectors.Length <= 0) return;
        _selectUnits = _selectUnits.Where(u => u.CurrentState != Scenes.Battle.UnitCharacter.State.CharacterUnitStateType.Dead).ToArray();
        float interval = LineLength(vectors) / _selectUnits.Length;
        if (interval < 0.5f) return;
        BattleFormationClac clac = new BattleFormationClac();
        var meleeUnits = _selectUnits.Where(u => u.WeaponType == UnitWeaponType.Melee).ToArray();
        var rangeUnits = _selectUnits.Where(u => u.WeaponType == UnitWeaponType.Range).ToArray();
        var formationPosition = clac.FormationCurve(meleeUnits.Select(u => u.Transform).ToArray(), rangeUnits.Select(u => u.Transform).ToArray(), vectors);

        for (int i = 0; i < meleeUnits.Length; i++)
        {
            meleeUnits[i].SetFormationPoint(formationPosition.meleePositions[i]).Forget();
        }
        for (int i = 0; i < rangeUnits.Length; i++)
        {
            rangeUnits[i].SetFormationPoint(formationPosition.rangePositions[i]).Forget();
        }
        _formationPosition.OnNext(formationPosition);
    }


    private float LineLength(Vector3[] vectors)
    {
        return vectors.Zip(vectors.Skip(1), Vector3.Distance).Sum();
    }

    private List<CharacterUnitModel> DetectUnitsInsideShape(Vector3[] linePositions)
    {

        // 内側にあるBallをフィルタリング
        return _playerUnits
            .Where(unit => IsPointInsidePolygon(unit.Transform.position, linePositions))
            .ToList();
    }

    /// <summary>
    /// 点がポリゴンの内側にあるかを判定（射影法）
    /// </summary>
    private bool IsPointInsidePolygon(Vector3 point, Vector3[] polygon)
    {
        int intersections = 0;
        Vector3 rayEnd = point + Vector3.right * 1000f; // 右方向に遠くまでRayを飛ばす

        for (int i = 0; i < polygon.Length; i++)
        {
            Vector3 v1 = polygon[i];
            Vector3 v2 = polygon[(i + 1) % polygon.Length];

            if (DoLinesIntersect(point, rayEnd, v1, v2))
            {
                intersections++;
            }
        }

        // 交差回数が奇数なら内側、偶数なら外側
        return (intersections % 2 == 1);
    }

    /// <summary>
    /// 2つの線分が交差するか判定
    /// </summary>
    private bool DoLinesIntersect(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
    {
        float d1 = Direction(q1, q2, p1);
        float d2 = Direction(q1, q2, p2);
        float d3 = Direction(p1, p2, q1);
        float d4 = Direction(p1, p2, q2);

        return (d1 * d2 < 0 && d3 * d4 < 0);
    }

    private float Direction(Vector3 a, Vector3 b, Vector3 c)
    {
        return (b.x - a.x) * (c.z - a.z) - (b.z - a.z) * (c.x - a.x);
    }

}
