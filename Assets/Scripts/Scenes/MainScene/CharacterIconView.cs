using UnityEngine;

namespace Scenes.MainScene
{
    public class CharacterIconView : MonoBehaviour
    {
        private Vector3 _offSetPosition = new Vector3(30, 30, 0);

        public void UpdateIconPosition(Transform transform)
        {
            this.transform.SetParent(transform,false);
            this.transform.localPosition = _offSetPosition;
        }
    }
}