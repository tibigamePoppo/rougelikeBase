using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Scenes.MainScene
{
    public class StagePresenter : MonoBehaviour
    {
        private StageModel _stageModel;
        private StageView _stageView;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            _stageModel = new StageModel();
            _stageView = GetComponent<StageView>();
            _stageModel.Init();
            _stageView.Init(_stageModel.UnitInfo);
            _stageView.OnDepthForword.Subscribe(d =>
            {
                _stageModel.NextDepth(d);
                _stageView.UnitUpdate(_stageModel.CurrentDepth);
            }).AddTo(this);
        }
    }
}