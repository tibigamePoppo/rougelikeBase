using UnityEngine;

namespace Scenes.MainScene.Relic
{
    public abstract class  RelicItemBase : MonoBehaviour
    {
        public int _relicItemId;
        public string _relicItemName;
        public Sprite _sprite;
        public string _effectText;
        public virtual void Init() { }
    }
}