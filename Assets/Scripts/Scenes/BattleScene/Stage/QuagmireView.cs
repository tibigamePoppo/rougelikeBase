using Scenes.Battle.UnitCharacter;
using UnityEngine;

namespace Scenes.Battle.Stage
{
    public class QuagmireView : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"in {other.gameObject.name}");
            if (other.TryGetComponent(out CharacterUnitView view))
            {
                view.SetDisorder(Disorder.Slow, false);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CharacterUnitView view))
            {
                view.SetDisorder(Disorder.Slow, true);
            }
        }
    }
}