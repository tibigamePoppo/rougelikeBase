using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

namespace Scenes.MenuScene
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField] private Button _returnButton;
        [SerializeField] private Button _seMuteButton;
        [SerializeField] private Button _bgmMuteButton;
        [SerializeField] private Slider _seSlider;
        [SerializeField] private Slider _bgmlider;
        [SerializeField] private Sprite[] _spikerSprite;
        void Start()
        {
            Init();
        }

        public void Init()
        {
            _bgmMuteButton.image.sprite = Config.bgmVolume != 0 ? _spikerSprite[0] : _spikerSprite[1];
            _seMuteButton.image.sprite = Config.seVolume != 0 ? _spikerSprite[0] : _spikerSprite[1];
            _seSlider.value = Config.seVolume;
            _bgmlider.value = Config.bgmVolume;
            _seMuteButton.OnClickAsObservable().Subscribe(_ =>
            {
                Config.seVolume = Config.seVolume != 0 ? 0 : 0.1f;
                _seSlider.value = Config.seVolume;
                _seMuteButton.image.sprite = Config.seVolume != 0 ? _spikerSprite[0] : _spikerSprite[1];
            }).AddTo(this);
            _bgmMuteButton.OnClickAsObservable().Subscribe(_ =>
            {
                Config.bgmVolume = Config.bgmVolume != 0 ? 0 : 0.1f;
                _bgmlider.value = Config.bgmVolume;
                _bgmMuteButton.image.sprite = Config.bgmVolume != 0 ? _spikerSprite[0] : _spikerSprite[1];
            }).AddTo(this);
            _seSlider.OnValueChangedAsObservable().Subscribe(v => Config.seVolume = v).AddTo(this);
            _bgmlider.OnValueChangedAsObservable().Subscribe(v => Config.bgmVolume = v).AddTo(this);
            _returnButton.OnClickAsObservable().Subscribe(_ => ReturnMenu()).AddTo(this);
        }

        private void ReturnMenu()
        {
            SceneManager.UnloadSceneAsync("MenuScene");
        }
    }
}