using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private TutorialStepBase[] _panelsBase;
    [SerializeField]

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (var step in _panelsBase)
        {
            step.Init();
        }
        TutorialStepAsync().Forget();
    }

    private async UniTaskVoid TutorialStepAsync()
    {
        foreach (var item in _panelsBase)
        {
            await item.TutorialAcition();
        }
    }

}
