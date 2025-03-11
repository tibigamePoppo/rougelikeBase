using System;
using System.Collections;
using System.Collections.Generic;
using Scenes.MainScene.Player;
using UniRx;
using UnityEngine;

namespace Scenes.Battle
{
    public class BattlePresenter : MonoBehaviour
    {
        private BattleView _view;
        private Subject<bool> _isPlayerWinBattle = new Subject<bool>();

        public IObservable<bool> IsPlayerWinBattle => _isPlayerWinBattle;

        public void Init(EnemyLevel enemyLevel,List<UnitData> playerCards,EnemyData[] preliminaryEnemyData = null)
        {
            _view = GetComponent<BattleView>();
            _view.Init(enemyLevel, playerCards, preliminaryEnemyData);
            _view.IsPlayerWinBattle.Subscribe(isWin => _isPlayerWinBattle.OnNext(isWin)).AddTo(this);
        }
    }
}