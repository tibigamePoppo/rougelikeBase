using UnityEngine;

namespace Scenes.MainScene.Relic
{
    public abstract class  RelicItemBase : MonoBehaviour
    {
        public int relicItemId;
        public string relicItemName;
        public Sprite sprite;
        public string effectText;
        public virtual void Init() { }
    }
}