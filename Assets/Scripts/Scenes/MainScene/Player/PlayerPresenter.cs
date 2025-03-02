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
            _singleton.SetCurrentDeck(_model.CurrentCardDataList);
            _view.Init(_model.CurrentPopularity, _model.CurrentMoney, _model.CurrentCardDataList);
            _model.OnPopularityChange.Subscribe(v => _view.UpdatePopularityText(v)).AddTo(this);
            _model.OnMoneyChange.Subscribe(v =>
            {
                _view.UpdateMoneyText(v);
                _singleton.SetCurrentMoney(v);
            }).AddTo(this);
            _view.OnDeckButtonClick.Subscribe(_ => _view.OpenDeckView(_model.CurrentCardDataList)).AddTo(this);

            _model.OnDeckChange.Subscribe(_ => _singleton.SetCurrentDeck(_model.CurrentCardDataList));
            _model.OnUpdateRelicItem.Subscribe(r => _singleton.SetCurrentRelic(r));
            _singleton.OnChangePopularityEvent.Subscribe(v => _model.ChangePopularity(v)).AddTo(this);
            _singleton.OnChangeMoneyEvent.Subscribe(v => _model.ChangeMoney(v)).AddTo(this);
            _singleton.OnAddCardEvent.Subscribe(c => _model.AddCard(c)).AddTo(this);
            _singleton.OnRemoveCardEvent.Subscribe(c => _model.RemoveCard(c)).AddTo(this);
            _singleton.OnAddRelicItemEvent.Subscribe(r => _model.AddRelicItem(r)).AddTo(this);
            _singleton.OnRemoveRelicItemEvent.Subscribe(r => _model.RemoveRelicItem (r)).AddTo(this);
        }
    }
}
