using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class TutorialStepBase : MonoBehaviour
{
    public virtual void Init() { }
    public virtual async UniTask TutorialAcition() { }
}
