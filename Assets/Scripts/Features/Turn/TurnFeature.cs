using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class TurnFeature : BaseVisualFeature<TurnVisual>, ITurn
    {
        // Action timing constants
        public const float SECONDS_PER_100_ACTION_POINTS = 5f;

        [Inject] public TurnRecord Record { get; set; }
        [Inject] public BattleUnitsRecord UnitsRecord { get; set; }
        [Inject] public IGridSelection GridSelection { get; set; }
        [Inject] public IBattleUnits BattleUnits { get; set; }
        

        private BaseFactory _turnBarFactory;
        private TurnBarVisual _turnBarVisual;

        public override void Bootstrap(IBootstrap bootstrap)
        {
            base.Bootstrap(bootstrap);
            
            if(bootstrap.Factories.TryGetValue(typeof(TurnBarVisual), out var factory))
            {
                _turnBarFactory = factory;
            }
        }

        public async UniTask Start()
        {
            await CreateVisual();
            _turnBarVisual = await _turnBarFactory.Create<TurnBarVisual>();
            _turnBarVisual.SetFeature(this);
            
            _visual.SetTurnData();
        }

        public void SelectedMyUnit()
        {
            var coordinate = GridSelection.GetSelectedCoordinate();
            var unitData = BattleUnits.GetUnitData(coordinate);
            var unitConfig = BattleUnits.GetUnitConfig(unitData.BattleUnitId);

            _turnBarVisual.ShowMyTurn(unitData.TurnOrder, unitConfig.ActionPoints);
        }

        public void OrderTurn(Vector2Int unitCoordinate, Vector2Int targetCoordinate, AbilityMode ability)
        {
            var unitData = BattleUnits.GetUnitData(unitCoordinate);
            var unitConfig = BattleUnits.GetUnitConfig(unitData.BattleUnitId);
            var moveAction = unitConfig.Actions.First(a => a.Ability == ability);

            // Calculate current total action points used
            int currentActionPointsUsed = unitData.TurnOrder.Actions.Sum(action => action.ActionPoints);

            // Check if adding this action would exceed the unit's action points limit
            int newActionPoints = moveAction.ActionPointsRequired;
            if (currentActionPointsUsed + newActionPoints > unitConfig.ActionPoints)
            {
                DJ.Play(DJ.Wrong_Sound);
                Notebook.NoteWarning($"Cannot order action: would exceed action points limit. Current: {currentActionPointsUsed}, New: {newActionPoints}, Limit: {unitConfig.ActionPoints}");
                return;
            }

            var newAction = new BattleUnitData.Action()
            {
                Ability = ability,
                ActionPoints = moveAction.ActionPointsRequired,
                Interception = moveAction.ActionInterception,
                Target = targetCoordinate,
                ActionPointStart = currentActionPointsUsed
            };

            unitData.TurnOrder.Actions.Add(newAction);

            _turnBarVisual.ShowMyTurn(unitData.TurnOrder, unitConfig.ActionPoints);
            DJ.Play(DJ.Click_Sound);
        }

        public void OnTurnClicked()
        {
            if (Record.InTurn)
            {
                return;
            }
            
            GridSelection.HandleRightClick();
            
            Record.InTurn = true;
            Record.Turn++;
            
            _visual.SetTurnData();

            foreach (var unit in UnitsRecord.BattleUnits)
            {
                if (unit.TurnOrder.Actions.Any())
                {
                    ExecuteTurn(unit, unit.TurnOrder).Forget();
                }
            }
            
            
        }

        private async UniTask ExecuteTurn(BattleUnitData unit, BattleUnitData.Turn turn)
        {
            Record.UnitsExecuting++;
            foreach (var action in turn.Actions)
            {
                if (action.Ability == AbilityMode.Move)
                {
                    await BattleUnits.ExecuteMove(unit.Coordinate, action.Target, action.ActionPoints);
                }
                else if (action.Ability == AbilityMode.Rotate)
                {
                    await BattleUnits.ExecuteRotate(unit.Coordinate, action.Target, action.ActionPoints);
                }
                else if (action.Ability == AbilityMode.Attack)
                {
                    await BattleUnits.ExecuteAttack(unit.Coordinate, action.Target, action.ActionPoints);
                }
            }
            Record.UnitsExecuting--;

            if (Record.UnitsExecuting == 0)
            {
                EndTurn();
            }
        }

        private void EndTurn()
        {
            foreach (var unit in UnitsRecord.BattleUnits)
            {
                unit.TurnOrder.Actions.Clear();
            }
            
            Record.InTurn = false;
            _visual.SetTurnData();
        }
    }
}