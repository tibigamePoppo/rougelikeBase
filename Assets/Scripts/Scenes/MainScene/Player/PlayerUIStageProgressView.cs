using System.Collections.Generic;
using UnityEngine;

namespace Scenes.MainScene.Player
{
    public class PlayerUIStageProgressView : MonoBehaviour
    {
        [SerializeField] ProgressSingleView _progressSingleView;
        private List<ProgressSingleView> _progressViews = new List<ProgressSingleView>();
        public void Init(int depth)
        {
            var startView = Instantiate(_progressSingleView, transform);
            startView.Init(ProgressType.Start);
            startView.NextProgress();
            _progressViews.Add(startView);
            for (int i = 1; i < depth; i++)
            {
                var view = Instantiate(_progressSingleView,transform);
                view.Init(ProgressType.Normal);
                _progressViews.Add(view);
            }
            var  bossView = Instantiate(_progressSingleView, transform);
            bossView.Init(ProgressType.Boss);
            _progressViews.Add(bossView);
        }

        public void UpdateProgress(int depth)
        {
            _progressViews[depth - 1].Progressed();
            _progressViews[depth].NextProgress();
        }

        public void ResetProgress(int depth)
        {
            foreach (var view in _progressViews)
            {
                Destroy(view.gameObject);
            }
            _progressViews.Clear();
            Init(depth);
        }

    }
}