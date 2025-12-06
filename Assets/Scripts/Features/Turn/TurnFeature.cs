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
        [Inject] public IGrid Grid { get; set; }
        

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
            _turnBarVisual.Clean();
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

            if (ability == AbilityMode.Move)
            {
                OrderMoveActions(unitData, unitConfig, unitCoordinate, targetCoordinate);
            }
            else if (ability == AbilityMode.Wait)
            {
                OrderWaitAction(unitData, unitConfig, unitCoordinate);
            }
            else
            {
                OrderSingleAction(unitData, unitConfig, unitCoordinate, targetCoordinate, ability);
            }

            _turnBarVisual.ShowMyTurn(unitData.TurnOrder, unitConfig.ActionPoints);
            DJ.Play(DJ.Click_Sound);
        }

        public void ResetSelectedUnitActions()
        {
            var coordinate = GridSelection.GetSelectedCoordinate();
            var unitData = BattleUnits.GetUnitData(coordinate);
            if (unitData != null)
            {
                unitData.TurnOrder.Actions.Clear();
                _turnBarVisual.ShowMyTurn(unitData.TurnOrder, BattleUnits.GetUnitConfig(unitData.BattleUnitId).ActionPoints);
                Notebook.NoteData("Cleared all actions for selected unit");
            }
        }

        private void OrderMoveActions(BattleUnitData unitData, BattleUnitConfig unitConfig, Vector2Int unitCoordinate, Vector2Int targetCoordinate)
        {
            // Calculate path to target
            var path = HexPathfinder.CalculatePath(Grid, unitCoordinate, targetCoordinate);

            if (!path.IsValid)
            {
                Notebook.NoteWarning($"No valid path found from {unitCoordinate} to {targetCoordinate}");
                DJ.Play(DJ.Wrong_Sound);
                return;
            }

            // Get the move action configuration
            var moveActionConfig = unitConfig.Actions.First(a => a.Ability == AbilityMode.Move);

            // Calculate current total action points used
            int currentActionPointsUsed = unitData.TurnOrder.Actions.Sum(action => action.ActionPoints);

            // Each grid movement step costs the configured action points
            int actionPointsPerMoveStep = moveActionConfig.ActionPointsRequired;

            // Add move actions for each step in the path
            for (int i = 0; i < path.TotalSteps; i++)
            {
                var step = path.Steps[i];

                // Check if adding this step would exceed the unit's action points limit
                if (currentActionPointsUsed + actionPointsPerMoveStep > unitConfig.ActionPoints)
                {
                    Notebook.NoteWarning($"Cannot add move step: would exceed action points limit. Added {i} steps out of {path.TotalSteps}");
                    DJ.Play(DJ.Wrong_Sound);
                    break;
                }

                var moveAction = new BattleUnitData.Action()
                {
                    Ability = AbilityMode.Move,
                    ActionPoints = actionPointsPerMoveStep,
                    Interception = moveActionConfig.ActionInterception,
                    Target = step.Coordinate,
                    ActionPointStart = currentActionPointsUsed
                };

                unitData.TurnOrder.Actions.Add(moveAction);
                currentActionPointsUsed += actionPointsPerMoveStep;
            }

            Notebook.NoteData($"Ordered {unitData.TurnOrder.Actions.Count(action => action.Ability == AbilityMode.Move)} move steps");
        }

        private void OrderWaitAction(BattleUnitData unitData, BattleUnitConfig unitConfig, Vector2Int unitCoordinate)
        {
            var actionConfig = unitConfig.Actions.First(a => a.Ability == AbilityMode.Wait);

            // Calculate current total action points used
            int currentActionPointsUsed = unitData.TurnOrder.Actions.Sum(action => action.ActionPoints);

            // Check if adding this action would exceed the unit's action points limit
            int newActionPoints = actionConfig.ActionPointsRequired;
            if (currentActionPointsUsed + newActionPoints > unitConfig.ActionPoints)
            {
                DJ.Play(DJ.Wrong_Sound);
                Notebook.NoteWarning($"Cannot order wait action: would exceed action points limit. Current: {currentActionPointsUsed}, New: {newActionPoints}, Limit: {unitConfig.ActionPoints}");
                return;
            }

            var newAction = new BattleUnitData.Action()
            {
                Ability = AbilityMode.Wait,
                ActionPoints = actionConfig.ActionPointsRequired,
                Interception = actionConfig.ActionInterception,
                Target = unitCoordinate, // Use unit's current coordinate as target (not used)
                ActionPointStart = currentActionPointsUsed
            };

            unitData.TurnOrder.Actions.Add(newAction);
        }

        private void OrderSingleAction(BattleUnitData unitData, BattleUnitConfig unitConfig, Vector2Int unitCoordinate, Vector2Int targetCoordinate, AbilityMode ability)
        {
            var actionConfig = unitConfig.Actions.First(a => a.Ability == ability);

            // Calculate current total action points used
            int currentActionPointsUsed = unitData.TurnOrder.Actions.Sum(action => action.ActionPoints);

            // Check if adding this action would exceed the unit's action points limit
            int newActionPoints = actionConfig.ActionPointsRequired;
            if (currentActionPointsUsed + newActionPoints > unitConfig.ActionPoints)
            {
                DJ.Play(DJ.Wrong_Sound);
                Notebook.NoteWarning($"Cannot order action: would exceed action points limit. Current: {currentActionPointsUsed}, New: {newActionPoints}, Limit: {unitConfig.ActionPoints}");
                return;
            }

            var newAction = new BattleUnitData.Action()
            {
                Ability = ability,
                ActionPoints = actionConfig.ActionPointsRequired,
                Interception = actionConfig.ActionInterception,
                Target = targetCoordinate,
                ActionPointStart = currentActionPointsUsed
            };

            unitData.TurnOrder.Actions.Add(newAction);
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
            
            WaitForAllUnitsToExecuteRoutine().Forget();
        }

        private async UniTask WaitForAllUnitsToExecuteRoutine()
        {
            while (true)
            {
                if (Record.UnitsExecuting == 0)
                {
                    EndTurn();
                    return;
                }
                await UniTask.Yield();
            }
        }

        private async UniTask ExecuteTurn(BattleUnitData unit, BattleUnitData.Turn turn)
        {
            Record.UnitsExecuting++;
            foreach (var action in turn.Actions)
            {
                if (action.Ability == AbilityMode.Move)
                {
                    await BattleUnits.ExecuteMove(unit.Coordinate, action.Target, action.ActionPoints, action.Interception);
                }
                else if (action.Ability == AbilityMode.Rotate)
                {
                    await BattleUnits.ExecuteRotate(unit.Coordinate, action.Target, action.ActionPoints);
                }
                else if (action.Ability == AbilityMode.Attack)
                {
                    await BattleUnits.ExecuteAttack(unit.Coordinate, action.Target, action.ActionPoints, action.Interception);
                }
                else if (action.Ability == AbilityMode.Wait)
                {
                    await BattleUnits.ExecuteWait(unit.Coordinate, action.ActionPoints, action.Interception);
                }
            }
            Record.UnitsExecuting--;
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