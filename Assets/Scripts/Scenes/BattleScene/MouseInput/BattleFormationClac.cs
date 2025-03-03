using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattleFormationClac
{
    public void FormationO(Transform[] meleeUnits,Transform[] rangeUnits, Vector3[] path)
    {
        float circumference = path.Zip(path.Skip(1), Vector3.Distance).Sum();
        float meleeInterval = circumference / (meleeUnits.Length - 1);
        float rangenterval = circumference / (rangeUnits.Length - 1);
        List<Vector3> meleeUnitsPoints = new List<Vector3>();
        List<Vector3> rangeUnitsPoints = new List<Vector3>();

        Vector3[] sumPlot = { path.OrderByDescending(v => v.x).First(), path.OrderBy(v => v.x).First(), path.OrderByDescending(v => v.y).First(), path.OrderBy(v => v.y).First() };
        Vector3 sum = Vector3.zero;
        foreach (var point in sumPlot)
        {
            sum += point; // 各点の座標を合計
        }
        Vector3 center = sum / path.Length;

        meleeUnitsPoints.Add(path[0]);
        rangeUnitsPoints.Add(path[0] - (path[0] - center) / 4);

        float remainingDistance = meleeInterval;
        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector3 start = path[i];
            Vector3 end = path[i + 1];
            float segmentLength = Vector3.Distance(start, end);

            while (remainingDistance <= segmentLength)
            {
                Vector3 point = Vector3.Lerp(start, end, remainingDistance / segmentLength);
                meleeUnitsPoints.Add(point);
                remainingDistance += rangenterval;
            }
            remainingDistance -= segmentLength;
        }
        remainingDistance = rangenterval;
        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector3 start = path[i];
            Vector3 end = path[i + 1];
            float segmentLength = Vector3.Distance(start, end);

            while (remainingDistance <= segmentLength)
            {
                Vector3 point = Vector3.Lerp(start, end, remainingDistance / segmentLength);
                rangeUnitsPoints.Add(point - (point - center) / 4);
                remainingDistance += rangenterval;
            }
            remainingDistance -= segmentLength;
        }

        meleeUnitsPoints.Add(path.Last());
        rangeUnitsPoints.Add(path.Last() - (path.Last() - center) / 4);
        SetPosition(meleeUnits, rangeUnits, meleeUnitsPoints, rangeUnitsPoints);
    }

    public void FormationI(Transform[] meleeUnits, Transform[] rangeUnits, Vector3[] path)
    {
        float length = path.Zip(path.Skip(1), Vector3.Distance).Sum();
        float meleeInterval = length / (meleeUnits.Length - 1);
        float rangenterval = length / (rangeUnits.Length - 1);
        List<Vector3> meleeUnitsPoints = new List<Vector3>();
        List<Vector3> rangeUnitsPoints = new List<Vector3>();

        Vector3 pathDirection = (path.Last() - path.First()).normalized;
        Vector3 normal = Vector3.up;
        Vector3 rightAngleDir = Vector3.Cross(normal, pathDirection).normalized;
        rightAngleDir = rightAngleDir * 3;

        meleeUnitsPoints.Add(path[0]);
        rangeUnitsPoints.Add(path[0] + rightAngleDir);

        float melleeRemainingDistance = meleeInterval;
        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector3 start = path[i];
            Vector3 end = path[i + 1];
            float segmentLength = Vector3.Distance(start, end);

            while (melleeRemainingDistance <= segmentLength)
            {
                Vector3 point = Vector3.Lerp(start, end, melleeRemainingDistance / segmentLength);
                meleeUnitsPoints.Add(point);
                melleeRemainingDistance += meleeInterval;
            }
            melleeRemainingDistance -= segmentLength;
        }
        float rangeRemainingDistance = rangenterval;
        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector3 start = path[i];
            Vector3 end = path[i + 1];
            float segmentLength = Vector3.Distance(start, end);

            while (rangeRemainingDistance <= segmentLength)
            {
                Vector3 point = Vector3.Lerp(start, end, rangeRemainingDistance / segmentLength);
                rangeUnitsPoints.Add(point + rightAngleDir);
                rangeRemainingDistance += rangenterval;
            }
            rangeRemainingDistance -= segmentLength;
        }

        meleeUnitsPoints.Add(path.Last());
        rangeUnitsPoints.Add(path.Last() + rightAngleDir);
        SetPosition(meleeUnits,rangeUnits,meleeUnitsPoints, rangeUnitsPoints);
    }

    public FormationPositionInfo FormationCurve(Transform[] meleeUnits, Transform[] rangeUnits, Vector3[] path)
    {
        float length = path.Zip(path.Skip(1), Vector3.Distance).Sum();
        float meleeInterval = length / (meleeUnits.Length - 1);
        float rangenterval = length / (rangeUnits.Length - 1);
        List<Vector3> meleeUnitsPoints = new List<Vector3>();
        List<Vector3> rangeUnitsPoints = new List<Vector3>();

        Vector3 pathDirection = (path[2] - path.First()).normalized;
        Vector3 normal = Vector3.up;
        Vector3 rightAngleDir = Vector3.Cross(normal, pathDirection).normalized;
        rightAngleDir = rightAngleDir * 3;

        meleeUnitsPoints.Add(path[0]);
        rangeUnitsPoints.Add(path[0] + rightAngleDir);

        float melleeRemainingDistance = meleeInterval;
        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector3 start = path[i];
            Vector3 end = path[i + 1];
            float segmentLength = Vector3.Distance(start, end);

            while (melleeRemainingDistance <= segmentLength)
            {
                Vector3 point = Vector3.Lerp(start, end, melleeRemainingDistance / segmentLength);
                meleeUnitsPoints.Add(point);
                melleeRemainingDistance += meleeInterval;
            }
            melleeRemainingDistance -= segmentLength;
        }

        float rangeRemainingDistance = rangenterval;
        for (int i = 0; i < path.Length - 1; i++)
        {
            Vector3 start = path[i];
            Vector3 end = path[i + 1];
            float segmentLength = Vector3.Distance(start, end);
            if (rangeRemainingDistance <= segmentLength)
            {
                pathDirection = path.Length > i ? (path[i + 1] - path[i - 1]).normalized : (path[i] - path[i - 1]).normalized;
                rightAngleDir = Vector3.Cross(normal, pathDirection).normalized;
                rightAngleDir = rightAngleDir * 3;
                while (rangeRemainingDistance <= segmentLength)
                {
                    Vector3 point = Vector3.Lerp(start, end, rangeRemainingDistance / segmentLength);
                    rangeUnitsPoints.Add(point + rightAngleDir);
                    rangeRemainingDistance += rangenterval;
                }
            }
            rangeRemainingDistance -= segmentLength;
        }

        meleeUnitsPoints.Add(path.Last());
        rangeUnitsPoints.Add(path.Last() + rightAngleDir);
        return SetPositionReturnPosition(meleeUnits, rangeUnits, meleeUnitsPoints, rangeUnitsPoints);
    }

    private void SetPosition(Transform[] meleeUnits, Transform[] rangeUnits, List<Vector3> meleePosition, List<Vector3> rangePosition)
    {
        Debug.Log($"meleeUnits {meleeUnits.Length},rangeUnits {rangeUnits.Length},meleePosition {meleePosition.Count},rangePosition {rangePosition.Count}");
        while (meleePosition.Count > meleeUnits.Length && meleePosition.Count > 0)
        {
            meleePosition.Remove(meleePosition.Last());
        }
        while (meleePosition.Count < meleeUnits.Length)
        {
            meleePosition.Add(meleePosition[meleePosition.Count - 1]);
        }

        while (rangePosition.Count > rangeUnits.Length && rangePosition.Count > 0)
        {
            rangePosition.Remove(rangePosition.Last());
        }
        while (rangePosition.Count < rangeUnits.Length)
        {
            rangePosition.Add(rangePosition[rangePosition.Count - 1]);
        }

        for (int i = 0; i < meleeUnits.Length; i++)
        {
            meleeUnits[i].position = meleePosition[i];
        }
        for (int i = 0; i < rangeUnits.Length; i++)
        {
            rangeUnits[i].position = rangePosition[i];
        }
    }
    private FormationPositionInfo SetPositionReturnPosition(Transform[] meleeUnits, Transform[] rangeUnits, List<Vector3> meleePosition, List<Vector3> rangePosition)
    {
        while (meleePosition.Count > meleeUnits.Length && meleePosition.Count > 0)
        {
            meleePosition.Remove(meleePosition.Last());
        }
        while (meleePosition.Count < meleeUnits.Length)
        {
            meleePosition.Add(meleePosition[meleePosition.Count - 1]);
        }

        while (rangePosition.Count > rangeUnits.Length && rangePosition.Count > 0)
        {
            rangePosition.Remove(rangePosition.Last());
        }
        while (rangePosition.Count < rangeUnits.Length)
        {
            rangePosition.Add(rangePosition[rangePosition.Count - 1]);
        }

        return new FormationPositionInfo( meleePosition, rangePosition);
    }
}

public class FormationPositionInfo
{
    public List<Vector3> meleePositions;
    public List<Vector3> rangePositions;
    public FormationPositionInfo(List<Vector3> m, List<Vector3> r)
    {
        meleePositions = m;
        rangePositions = r;
    }

}
