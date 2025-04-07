using UnityEngine;
using UniRx;

namespace Scenes.Title
{
    public class MasterDataLoadPresenter : MonoBehaviour
    {
        MasterDataLoadModel _model;
        MasterDataLoadView _view;
        void Start()
        {
            Init();
        }

        public void Init()
        {
            _model = new MasterDataLoadModel();
            _view = GetComponent<MasterDataLoadView>();
            _model.Init();
            _view.Init();
            _view.OnLoadMasterData.Subscribe(_ => _model.LoadMasterDataProcess()).AddTo(this);
            _model.OnProgress.Subscribe(p => _view.UpdateProgress(p)).AddTo(this);
        }
    }
}