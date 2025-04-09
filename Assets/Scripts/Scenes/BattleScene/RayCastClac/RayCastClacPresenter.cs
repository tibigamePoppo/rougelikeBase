using UnityEngine;
namespace Scenes.Battle.RayCastClac
{
    public class RayCastClacPresenter : MonoBehaviour
    {
        private RayCastClacView _view;
        private RayCastClacModel _model;

        void Start()
        {
            Init();
        }

        void Init()
        {
            _model = new RayCastClacModel();
            _view = GetComponent<RayCastClacView>();
            _view.Init();
            _model.Init();
        }
    }
}