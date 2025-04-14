using UnityEngine;
namespace Scenes.MainScene.Relic.Item
{
    public class CountEffect : RelicItemBase
    {
        [SerializeField] private int _count;
        private int currentCount;
        public int Count {  get { return currentCount; } }
        public override void Init()
        {
            currentCount = _count;
            PlayerSingleton.Instance.AddRelicItem(this);
            base.Init();
            Debug.Log($"Init {isEffect}, {currentCount}");
        }

        public override void OnEffect()
        {
            if (!isEffect) return;
            currentCount = Mathf.Max(0, currentCount - 1);
            Debug.Log($"OnEffect {currentCount}");
            if (currentCount == 0)
            {
                isEffect = false;
                Debug.Log($"isEffect {isEffect}");
            }
        }
    }
}