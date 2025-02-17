using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class RewardItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Image _image;
    private Button _button;

    public void Init(Reward reward)
    {
        _name.text = reward.name;
        _image.sprite = reward.sprite;
        _button = GetComponent<Button>();
        _button.OnClickAsObservable().Subscribe(_ =>
        {
            reward.action.ItemAction();
            Destroy(gameObject);
        }).AddTo(this);
    }
}

