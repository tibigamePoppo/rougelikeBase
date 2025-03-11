using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

namespace Scenes.Battle
{
    public class OnBattleFormationView : MonoBehaviour
    {
        [SerializeField] private Button _chargeButton;

        private Subject<Unit> _charge = new Subject<Unit>();

        public IObservable<Unit> OnCharge => _charge;

        public void Init()
        {
            gameObject.SetActive(false);
            _chargeButton.OnClickAsObservable().Subscribe(_ => _charge.OnNext(default)).AddTo(this);
        }

        public void Active()
        {
            gameObject.SetActive(true);
        }

    }
}