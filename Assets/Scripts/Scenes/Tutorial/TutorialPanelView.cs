using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TutorialPanelView : MonoBehaviour
{
    public void Init()
    {
        gameObject.SetActive(false);
    }

    public async UniTask ActivePanel()
    {
        gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromMilliseconds(1));
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space));
        gameObject.SetActive(false);
    }

}
