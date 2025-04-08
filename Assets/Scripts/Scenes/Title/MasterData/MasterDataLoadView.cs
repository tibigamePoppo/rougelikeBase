using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using System;
using Cysharp.Threading.Tasks;

namespace Scenes.Title
{
    public class MasterDataLoadView : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Image _progressBar;
        [SerializeField] private TextMeshProUGUI _progressText;

        private Subject<Unit> loadMasterData = new Subject<Unit>();
        public IObservable<Unit> OnLoadMasterData => loadMasterData;
        public void Init()
        {
            _progressText.text = "0%";
            _progressBar.fillAmount = 0;
            //gameObject.SetActive(false);
            _closeButton.OnClickAsObservable().Subscribe(_ => gameObject.SetActive(false)).AddTo(this);
            _loadButton.OnClickAsObservable().Subscribe(_ => loadMasterData.OnNext(default)).AddTo(this);
        }

        public void UpdateProgress(int progress)
        {
            _progressText.text = $"{progress}%";
            _progressBar.fillAmount = (float)progress / 100;
        }
    }
}