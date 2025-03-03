using UnityEngine;
using UniRx;

namespace Scenes.MainScene.Upgrade
{
    public class UnitUpgradePresenter : MonoBehaviour
    {
        private UnitUpgradeView _view;
        private UnitUpgradeModel _model; 

        public void Init()
        {
            _model = new UnitUpgradeModel();
            _view = GetComponent<UnitUpgradeView>();
            _model.Init();
            _view.Init();
            _view.InstanceBaseView(_model.UpgradableUnits());
            _model.OnUpdateChoices.Subscribe(c => _view.DisplayUpgradeDialog(c)).AddTo(this);
            _view.OnUpdateUnit.Subscribe(u => _model.UpdateUnit(u)).AddTo(this);
            _view.OnBaseSelectUnit.Subscribe(u => _model.BeginUpgradeUnit(u)).AddTo(this);
        }
    }
}