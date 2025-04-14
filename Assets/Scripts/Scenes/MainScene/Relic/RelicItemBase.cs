using UnityEngine;

namespace Scenes.MainScene.Relic
{
    public abstract class  RelicItemBase : MonoBehaviour
    {
        public int relicItemId;
        public string relicItemName;
        public Sprite sprite;
        public string effectText;
        public int shopCost;
        public bool isEffect;
        public virtual void Init() { isEffect = true; }
        public virtual void OnEffect()
        {
        }
    }
}