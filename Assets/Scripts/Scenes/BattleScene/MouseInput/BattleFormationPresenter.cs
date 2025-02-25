using UnityEngine;
using UniRx;
using Scenes.Battle.UnitCharacter;

public class BattleFormationPresenter : MonoBehaviour
{
    private BattleFormationView _view;
    private BattleFormationModel _model;

    public void Init(CharacterUnitModel[] unitModels)
    {
        _model = new BattleFormationModel();
        _view = GetComponent<BattleFormationView>();
        _model.Init(unitModels);
        _view.Init();
        _view.OnDrawSelectLine.Subscribe(s => _model.SetSelectLine(s)).AddTo(this);
        _view.OnDrawFormationLine.Subscribe(l => _model.SetFormationLine(l)).AddTo(this);
        _model.OnFormationChange.Subscribe(f => _view.SetFormation(f)).AddTo(this);

    }
}
