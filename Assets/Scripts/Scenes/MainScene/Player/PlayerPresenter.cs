using UnityEngine;
using UniRx;

namespace Scenes.MainScene.Player
{
    public class PlayerPresenter : MonoBehaviour
    {
        private PlayerModel _model;
        private PlayerUIView _view;
        private PlayerSingleton _singleton;
        void Start()
        {
            Init();
        }

        public void Init()
        {
            _model = new PlayerModel();
            _view = GetComponent<PlayerUIView>();
            _singleton = GetComponent<PlayerSingleton>();
            _model.Init();
            _view.Init(_model.CurrentHp,_model.CurrentMoney);
            _model.OnHpChange.Subscribe(v => _view.UpdateHpText(v)).AddTo(this);
            _model.OnMoneyChange.Subscribe(v => _view.UpdateMoneyText(v)).AddTo(this);
            _view.OnDeckButtonClick.Subscribe(_ => _view.OpenDeckView(_model.CurrentCards)).AddTo(this);

            _singleton.OnChangeHpEvent.Subscribe(v => _model.ChangeHp(v)).AddTo(this);
            _singleton.OnChangeMoneyEvent.Subscribe(v => _model.ChangeMoney(v)).AddTo(this);
            _singleton.OnAddCardEvent.Subscribe(c => _model.AddCard(c)).AddTo(this);
        }
    }
}
