using Scenes.MainScene.Cards;
using Scenes.MainScene.Player;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class RewordSelectCardView : MonoBehaviour
{
    [SerializeField] private CardView _cardView;

    public void Init(UnitData[] units)
    {
        Debug.Log($"unit length {units.Length}");
        for (int i = 0; i < units.Length; i++)
        {
            var unitData = units[i];
            var unit = Instantiate(_cardView, transform);
            unit.Init(unitData.status);
            var button = unit.gameObject.AddComponent<Button>();
            button.OnClickAsObservable().Subscribe(_ =>
            {
                PlayerSingleton.Instance.AddCard(unitData);
                gameObject.SetActive(false);
            }).AddTo(this);
        }
        gameObject.SetActive(true);
    }
}
