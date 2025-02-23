using System.Collections;
using System.Collections.Generic;
using Scenes.MainScene.Player;
using UnityEngine;

namespace Scenes.Battle
{
    public class BattlePresenter : MonoBehaviour
    {
        private BattleView _view;

        public void Init(EnemyLevel enemyLevel,List<UnitData> playerCards)
        {
            _view = GetComponent<BattleView>();
            _view.Init(enemyLevel, playerCards);
        }
    }
}