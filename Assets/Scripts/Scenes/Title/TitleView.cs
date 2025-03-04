using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

public class TitleView : MonoBehaviour
{
    [SerializeField] private Button _mainSceneButton;
    [SerializeField] private Button _tutorialSceneButton; 
    void Start()
    {
        Init();
    }

    private void Init()
    {
        _mainSceneButton.OnClickAsObservable().Subscribe(_ => SceneManager.LoadScene("MainScene")).AddTo(this);
        _tutorialSceneButton .OnClickAsObservable().Subscribe(_ => SceneManager.LoadScene("TutorialMainScene")).AddTo(this);
    }

}
