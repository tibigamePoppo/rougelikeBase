using System.Collections.Generic;
using Scenes.MainScene.Cards;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using System.Linq;
using Scenes.MainScene.Player;

namespace Scenes.MainScene.Upgrade
{
    public class UnitUpgradeView : MonoBehaviour
    {
        [SerializeField] private CardView _unitView;
        [SerializeField] private Transform _unitBaseTransform;
        [SerializeField] private CardView[] _upgradeUnits = new CardView[2];
        [SerializeField] private Button _windowClose;
        private List<UnitStatus> _instatiatedView = new List<UnitStatus>();
        private Dictionary<string, GameObject> _instantiatedCardView = new Dictionary<string, GameObject>();

        
        private UnitStatus _unitStatus1;
        private UnitStatus _unitStatus2;
        private Subject<string> _updateUnit = new Subject<string>();
        private Subject<string> _baseSelectUnit = new Subject<string>();

        public IObservable<string> OnUpdateUnit => _updateUnit;
        public IObservable<string> OnBaseSelectUnit => _baseSelectUnit;

        public void Init()
        {
            gameObject.SetActive(false);
            _upgradeUnits[0].gameObject.SetActive(false);
            _upgradeUnits[1].gameObject.SetActive(false);
            _windowClose.OnClickAsObservable().Subscribe(_ => HideUpgradeUnitsPanel()).AddTo(this);

            _upgradeUnits[0].GetComponent<Button>().OnClickAsObservable().Subscribe(_ =>
            {
                _updateUnit.OnNext(_unitStatus1.name);
                HideUpgradeUnitsPanel();
            }).AddTo(this);
            _upgradeUnits[1].GetComponent<Button>().OnClickAsObservable().Subscribe(_ =>
            {
                _updateUnit.OnNext(_unitStatus2.name);
                HideUpgradeUnitsPanel();
            }).AddTo(this);
            
        }

        private void HideUpgradeUnitsPanel()
        {
            gameObject.SetActive(false);
            _upgradeUnits[0].gameObject.SetActive(false);
            _upgradeUnits[1].gameObject.SetActive(false);
        }

        public void OpenPanel()
        {
            var playerDeck = PlayerSingleton.Instance.CurrentDeck;
            foreach (var cardView in _instantiatedCardView)
            {
                cardView.Value.SetActive(playerDeck.Any(p => p.status.name == cardView.Key));
            }
            
            gameObject.SetActive(true);
        }

        public void DisplayUpgradeDialog(UnitStatus[] updateUnit)
        {
            _upgradeUnits[0].gameObject.SetActive(true);
            _upgradeUnits[1].gameObject.SetActive(true);
            _unitStatus1 = updateUnit[0];
            _unitStatus2 = updateUnit[1];
            _upgradeUnits[0].Init(_unitStatus1);
            _upgradeUnits[1].Init(_unitStatus2);
        }

        public void InstanceBaseView(UnitStatus[] status)
        {
            var unInstanceUnit = status.Except(_instatiatedView).ToArray();
            if (unInstanceUnit.Length <= 0) return;

            foreach (var unit in unInstanceUnit)
            {
                var instance = Instantiate(_unitView, _unitBaseTransform);
                instance.Init(unit);
                _instatiatedView.Add(unit);
                _instantiatedCardView.Add(unit.name, instance.gameObject);
                var button = instance.gameObject.AddComponent<Button>();
                button.OnClickAsObservable().Subscribe(_ => _baseSelectUnit.OnNext(unit.name)).AddTo(this);
            }
        }
    }
}