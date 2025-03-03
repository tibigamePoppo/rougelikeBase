using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Scenes.Battle.UnitCharacter;
using UnityEngine;
using UniRx;
using System;
using System.Threading;

public class BattleFormationView : MonoBehaviour
{
    [SerializeField] private LineRenderer _selectLineRenderer;
    [SerializeField] private LineRenderer _formationLineRenderer;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private Material meshMaterial;  // Meshの表示用マテリアル
    [SerializeField] private Transform _arrowCanvas;
    [SerializeField] private GameObject _arrowImage;
    [SerializeField] private Camera _battleSceneCamera;

    private Mesh mesh;
    private MeshCollider _meshCollider;
    private Vector3 _lastMousePosition;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private List<GameObject> _arrowInstanceList = new List<GameObject>();
    private CancellationTokenSource _source;

    private Subject<CharacterUnitView[]> _selectCharacterUnits = new Subject<CharacterUnitView[]>();
    private Subject<Vector3[]> _selectLine = new Subject<Vector3[]>();
    private Subject<Vector3[]> _formationLine = new Subject<Vector3[]>();

    public IObservable<CharacterUnitView[]> OnSelectCharacterUnits => _selectCharacterUnits;
    public IObservable<Vector3[]> OnDrawSelectLine => _selectLine;
    public IObservable<Vector3[]> OnDrawFormationLine => _formationLine;

    public void Init()
    {
        _selectLineRenderer.positionCount = 0;
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
        _meshRenderer.material = meshMaterial;
        _source = new CancellationTokenSource();
        StartMouse1Check(_source.Token).Forget();
        StartMouse2Check(_source.Token).Forget();
    }


    private async UniTaskVoid StartMouse1Check(CancellationToken token)
    {
        while (true)
        {
            await UniTask.WaitUntil(() => Input.GetKey(KeyCode.Mouse1),cancellationToken: token);
            if (token.IsCancellationRequested) break;

            ResetSelectRenderer();
            while (Input.GetKey(KeyCode.Mouse1))
            {
                AddEdgeLine(_selectLineRenderer);
                await UniTask.WaitForFixedUpdate(cancellationToken: token);
            }

            if (token.IsCancellationRequested) break;
            if (_selectLineRenderer.positionCount < 1) continue;

            FixCircleLine();
            CreateMeshFromLineRenderer();

            Vector3[] points = new Vector3[_selectLineRenderer.positionCount];
            _selectLineRenderer.GetPositions(points);
            _selectLine.OnNext(points);

            await UniTask.WaitForFixedUpdate();
        }
    }

    private async UniTaskVoid StartMouse2Check(CancellationToken token)
    {
        while (true)
        {
            await UniTask.WaitUntil(() => Input.GetKey(KeyCode.Mouse0), cancellationToken: token);
            if (token.IsCancellationRequested) break;

            ResetFormationRenderer();
            while (Input.GetKey(KeyCode.Mouse0))
            {
                AddEdgeLine(_formationLineRenderer);
                await UniTask.WaitForFixedUpdate(cancellationToken: token);
            }

            if (token.IsCancellationRequested) break;
            if (_formationLineRenderer.positionCount < 1) continue;

            Vector3[] points = new Vector3[_formationLineRenderer.positionCount];
            _formationLineRenderer.GetPositions(points);
            _formationLine.OnNext(points);
            await UniTask.WaitForFixedUpdate();
        }
    }

    private void ResetSelectRenderer() 
    {
        _meshFilter.mesh = new Mesh();
        _selectLineRenderer.positionCount = 0;
    }

    private void ResetFormationRenderer()
    {
        _formationLineRenderer.positionCount = 0;
        for (int i = 0; i < _arrowInstanceList.Count; i++)
        {
            Destroy(_arrowInstanceList[i]);
        }
        _arrowInstanceList.Clear();
    }
     
    private void FixCircleLine()
    {
        if (Vector3.Distance(_selectLineRenderer.GetPosition(0), _selectLineRenderer.GetPosition(_selectLineRenderer.positionCount - 1)) < 1.5f)
        {
            _selectLineRenderer.positionCount += 1;
            _selectLineRenderer.SetPosition(_selectLineRenderer.positionCount - 1, _selectLineRenderer.GetPosition(0));
        }
    }

    private void AddEdgeLine(LineRenderer renderer)
    {
        Vector3 terrainPos;
        if (GetTerrainPosition(out terrainPos))
        {
            if(_lastMousePosition != null)
            {
                if (_lastMousePosition == terrainPos) return;
            }

            renderer.positionCount += 1;
            renderer.SetPosition(renderer.positionCount - 1, terrainPos);
            _lastMousePosition = terrainPos;

        }
    }

    private bool GetTerrainPosition(out Vector3 terrainPos)
    {
        terrainPos = Vector3.zero;

        Vector3 mousePos = Input.mousePosition;

        Ray ray = _battleSceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))
        {
            terrainPos = hit.point;
            terrainPos.y = 0.3f;
            return true;
        }

        return false;
    }


    private void CreateMeshFromLineRenderer()
    {
        Vector3[] points = new Vector3[_selectLineRenderer.positionCount];
        _selectLineRenderer.GetPositions(points);

        mesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Length + 1];
        int[] triangles = new int[(points.Length - 1) * 3];

        Vector3 center = points.Aggregate(Vector3.zero, (acc, p) => acc + p) / points.Length;
        vertices[0] = center;

        for (int i = 0; i < points.Length; i++)
        {
            vertices[i + 1] = points[i];
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        triangles[(points.Length - 1) * 3 - 3] = 0;
        triangles[(points.Length - 1) * 3 - 2] = points.Length;
        triangles[(points.Length - 1) * 3 - 1] = 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }



    public void SetFormation(FormationPositionInfo info)
    {
        var positions = info.rangePositions.Concat(info.meleePositions);
        foreach (var position in positions)
        {
            var arrow = Instantiate(_arrowImage, position, Quaternion.Euler(90,0,0), _arrowCanvas);
            _arrowInstanceList.Add(arrow);
        }
    }

    private void OnDestroy()
    {
        _source.Cancel();
        _source.Dispose();
    }
}