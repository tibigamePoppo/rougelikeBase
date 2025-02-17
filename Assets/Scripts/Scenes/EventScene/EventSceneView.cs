using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using System;

namespace Scenes.EventScene
{
    public class EventSceneView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Transform _buttonPanel;
        private Subject<EventEffectArg> _emitEvent = new Subject<EventEffectArg>();
        public IObservable<EventEffectArg> OnEmitEvent => _emitEvent;

        public void Init(EventBase[] eventArray)
        {
            foreach (var eventUnit in eventArray)
            {
                var button = Instantiate(_button, _buttonPanel);
                button.OnClickAsObservable().Subscribe(_ => _emitEvent.OnNext(eventUnit.effectArg)).AddTo(this);
                button.transform.GetComponentInChildren<TextMeshProUGUI>().text = eventUnit.buttonText;
            }
        }
    }
}
