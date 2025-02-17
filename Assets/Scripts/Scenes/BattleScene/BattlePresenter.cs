using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Battle
{
    public class BattlePresenter : MonoBehaviour
    {
        private BattleView _view;

        public void Init(EnemyLevel enemyLevel)
        {
            _view = GetComponent<BattleView>();
            _view.Init(enemyLevel);
        }
    }
}