using UnityEngine;
using UniRx;
using Scenes.Battle.UnitCharacter;

public class BattleFormationPresenter : MonoBehaviour
{
    private BattleFormationView _view;
    private BattleFormationModel _model;
    public bool isStarted = false;

    public void Init(CharacterUnitModel[] unitModels)
    {
        _model = new BattleFormationModel();
        _view = GetComponent<BattleFormationView>();
        _model.Init(unitModels);
        _view.Init();
        _view.OnDrawSelectLine.Where(_ => isStarted).Subscribe(s => _model.SetSelectLine(s)).AddTo(this);
        _view.OnDrawFormationLine.Where(_ => isStarted).Subscribe(l => _model.SetFormationLine(l)).AddTo(this);
        _model.OnFormationChange.Where(_ => isStarted).Subscribe(f => _view.SetFormation(f)).AddTo(this);

    }
}
