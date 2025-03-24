using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Scenes.Battle.UnitCharacter
{
    public class DamagePopUp : MonoBehaviour
    {
        private const float ANIMATIONTIME = 0.5f;
        [SerializeField] private TextMeshProUGUI _damagePopup;
        public void Init(DamageArg value)
        {
            _damagePopup.text = value.damage.ToString();
            _damagePopup.color = value.color;
            if (value.damageAnimation == DamageAnimation.Jump) JumpAnimation();
            else if (value.damageAnimation == DamageAnimation.Shake) ShakeAnimation();

        }

        private void JumpAnimation()
        {
            transform.DOLocalJump(Vector3.zero, -40, 1, ANIMATIONTIME).SetEase(Ease.InOutSine);
            transform.DOScale(transform.localScale * 1.1f, ANIMATIONTIME / 2).SetEase(Ease.InSine).SetLoops(2, LoopType.Yoyo);
            Destroy(this.gameObject, ANIMATIONTIME + 0.1f);
        }

        private void ShakeAnimation()
        {
            transform.DOPunchPosition(new Vector3(10f, 0, 0), ANIMATIONTIME, 20, 1f);
            Destroy(this.gameObject, ANIMATIONTIME + 0.1f);
        }
    }
}