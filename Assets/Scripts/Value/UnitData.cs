using Scenes.Battle.UnitCharacter;
using UnityEngine;

namespace Scenes.MainScene.Player
{
    [CreateAssetMenu(fileName = "NewUnitData", menuName = "ResourceValue/CardData")]
    public class UnitData : ScriptableObject
    {
        public UnitStatus status;
        public CharacterUnitPresenter prefab;
        public int shopCost;
    }
}