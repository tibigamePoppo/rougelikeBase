using UnityEngine;
using UniRx;

namespace Scenes.MainScene
{
    public class UnitLineView : MonoBehaviour
    {
        private Vector3 offset = new Vector3(0, 0, 0);
        public void Init(Transform positionA, Transform positionB)
        {
            var lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, positionA.position);
            lineRenderer.SetPosition(1, positionB.position);

            Observable.EveryUpdate()
                .Select(_ => positionA.position)
                .DistinctUntilChanged()
                .Subscribe(p =>
                {
                    lineRenderer.SetPosition(0, positionA.position + offset);
                }).AddTo(this);
            Observable.EveryUpdate()
                .Select(_ => positionB.position)
                .DistinctUntilChanged()
                .Subscribe(p =>
                {
                    lineRenderer.SetPosition(1, positionB.position + offset);
                }).AddTo(this);
        }

    }
}