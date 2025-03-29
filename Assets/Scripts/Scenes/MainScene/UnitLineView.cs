using UnityEngine;
using UniRx;

namespace Scenes.MainScene
{
    public class UnitLineView : MonoBehaviour
    {
        private Vector3 _offset = new Vector3(0, 0, 0);
        private Vector3[] _noiseOffsets;
        private const int POINTCOUNT = 3;
        private const float NOISEAMOUNT = 0.5f;

        private Transform _positionA;
        private Transform _positionB;
        private LineRenderer _lineRenderer;

        public void Init(Transform positionA, Transform positionB)
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _positionA = positionA;
            _positionB = positionB;

            _noiseOffsets = new Vector3[POINTCOUNT];
            for (int i = 1; i < POINTCOUNT - 1; i++) // 両端は固定
            {
                _noiseOffsets[i] = new Vector3(
                    Random.Range(-NOISEAMOUNT, NOISEAMOUNT),
                    Random.Range(-NOISEAMOUNT, NOISEAMOUNT),
                    0
                );
            }

            UpdateLine();

            
            Observable.EveryUpdate()
                .Select(_ => positionA.position)
                .DistinctUntilChanged()
                .Subscribe(p =>
                {
                    _lineRenderer.SetPosition(0, positionA.position + _offset);
                    UpdateLine();
                }).AddTo(this);
            Observable.EveryUpdate()
                .Select(_ => positionB.position)
                .DistinctUntilChanged()
                .Subscribe(p =>
                {
                    _lineRenderer.SetPosition(3, positionB.position + _offset);
                    UpdateLine();
                }).AddTo(this);
            
        }

        private void UpdateLine()
        {
            if (_positionA == null || _positionB == null) return;

            var points = new Vector3[POINTCOUNT];
            for (int i = 0; i < POINTCOUNT; i++)
            {
                float t = (float)i / (POINTCOUNT - 1); // 0 〜 1 の補間値
                points[i] = Vector3.Lerp(_positionA.position + _offset, _positionB.position + _offset, t) + _noiseOffsets[i];
            }
            _lineRenderer.SetPositions(points);
        }
    }
}