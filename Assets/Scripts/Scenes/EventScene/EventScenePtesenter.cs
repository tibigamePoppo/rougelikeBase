using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.EventScene
{
    public class EventScenePtesenter : MonoBehaviour
    {
        private EventSceneModel _model;
        private EventSceneView _view;

        public void Init(int depth)
        {
            _model = new EventSceneModel();
            _view = GetComponent<EventSceneView>();
            _model.Init();
            _view.Init(_model.SceneEvent, depth);
            _view.OnEmitEvent.Subscribe(e => _model.EventProcess(e)).AddTo(this);
            _model.ChangeScene.Subscribe(s => _view.ChangeScene(s)).AddTo(this);
        }
    }
}
