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
        void Start()
        {
            Init();
        }

        public void Init()
        {
            _model = new EventSceneModel();
            _view = GetComponent<EventSceneView>();
            _model.Init();
            _view.Init(_model.SceneEvent);
            _view.OnEmitEvent.Subscribe(e => _model.EventProcess(e)).AddTo(this);
        }
    }
}
