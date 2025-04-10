using UnityEngine;

namespace Common
{
    public class SingletonAbstract<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance = null;

        private void Awake()
        {
            if (Instance is null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(Instance);
                Instance = this as T;
            }
        }
    }
}