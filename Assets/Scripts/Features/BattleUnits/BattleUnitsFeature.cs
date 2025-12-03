using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class BattleUnitsFeature : BaseVisualFeature<BattleUnitsVisual>, IBattleUnits, IAppLaunchAgent, IBattleLaunchAgent
    {
        [Inject] public BattleUnitsRecord Record { get; set; }
        [Inject] public ILocalConfigService ConfigService { get; set; }
        [Inject] public IGrid Grid { get; set; }
        [Inject] public IGridSelection GridSelection { get; set; }
        [Inject] public IPlayerAccount PlayerAccount { get; set; }
        [Inject] public IHud Hud { get; set; }

        private BattleUnitsConfig _config;
        private BattleUnitsAssetPack _assetPack;

        public BattleUnitsAssetPack AssetPack => _assetPack;

        public async UniTask AppLaunch()
        {
            await CreateVisual();

            _config = ConfigService.GetConfig<BattleUnitsConfig>();
            _assetPack = await Summoner.SummoningService.LoadAssetPack<BattleUnitsAssetPack>();
        }

        public UniTask BattleLaunch()
        {
            Record.BattleUnits.Clear();

            // Load predetermined units from grid data
            LoadPredeterminedUnits();

            return UniTask.CompletedTask;
        }
        
        private void LoadPredeterminedUnits()
        {
            var gridData = Grid.GetGridData();

            if (gridData.PredeterminedUnits == null || gridData.PredeterminedUnits.Length == 0)
            {
                Notebook.NoteData("No predetermined units found in grid data");
                return;
            }

            foreach (var unitData in gridData.PredeterminedUnits)
            {
                // Get unit config to get proper health value
                var unitConfig = _config.GetBattleUnit(unitData.UnitId);
                if (unitConfig == null)
                {
                    Notebook.NoteWarning($"Unit config not found for {unitData.UnitId}, skipping");
                    continue;
                }

                // Validate that the spawn location is valid
                if (!Grid.IsValidForAbility(AbilityMode.Spawn, unitData.Coordinate))
                {
                    Notebook.NoteWarning($"Invalid spawn location for {unitData.UnitId} at {unitData.Coordinate}, skipping");
                    continue;
                }

                // Create the battle unit data
                var battleUnitData = new BattleUnitData()
                {
                    BattleUnitId = unitData.UnitId,
                    Coordinate = unitData.Coordinate,
                    Direction = unitData.Direction,
                    Health = unitConfig.MaxHealth,
                    Level = unitData.Level,
                    IsDead = false,
                    PlayerId = unitData.PlayerId
                };

                Record.BattleUnits.Add(battleUnitData);
                Notebook.NoteData($"Loaded predetermined unit: {unitData.UnitId} (Lv.{unitData.Level}) for {unitData.PlayerId} at {unitData.Coordinate}");
            }

            // Update hex ownership indicators after loading all units
            GridSelection.UpdateHexOwnershipIndicators();

            Notebook.NoteData($"Loaded {Record.BattleUnits.Count} predetermined battle units");
        }

        public void SpawnAllUnits()
        {
            foreach (var unitData in Record.BattleUnits)
            {
                _visual.SpawnUnit(unitData);
            }
        }

        public void DespawnAllUnits()
        {
            _visual.DespawnAllUnits();
        }
        
        public void UpdateUnitSelection(Vector2Int? coordinate)
        {
            ClearUnitSelection();
            
            if (coordinate == null)
            {
                return;
            }
            
            // Find unit at the coordinate (keep visual internal)
            var unitAtCoordinate = _visual.GetUnitAtCoordinate(coordinate.Value);
            
            // Select unit at new coordinate if one exists
            if (unitAtCoordinate != null)
            {
                unitAtCoordinate.SetGlow(true);
            }
        }
        
        public BattleUnitData GetUnitData(Vector2Int coordinate)
        {
            return Record.BattleUnits.Find(unit => unit.Coordinate == coordinate);
        }

        public BattleUnitConfig GetUnitConfig(string unitId)
        {
            return _config.GetBattleUnit(unitId);
        }
        
        public async UniTask ExecuteAttack(Vector2Int attackerCoordinate, Vector2Int targetCoordinate)
        {
            var attackerUnit = _visual.GetUnitAtCoordinate(attackerCoordinate);

            if (attackerUnit == null)
            {
                Notebook.NoteError("Attack failed: No attacker unit found");
                return;
            }

            // Get the unit data to get current direction
            var unitData = GetUnitData(attackerCoordinate);
            if (unitData != null)
            {
                // Execute rotation (in parallel with attack)
                // The rotator will calculate the target direction and return it
                attackerUnit
                    .Rotate(attackerCoordinate, targetCoordinate, unitData.Direction)
                    .ContinueWith(newDirection =>
                    {
                        unitData.Direction = newDirection;
                    })
                    .Forget();
            }

            // Execute attack (in parallel with rotation)
            await attackerUnit.Attack();

            // Check for enemy in facing direction and deal damage
            if (unitData != null)
            {
                DealDamageToTarget(attackerCoordinate, unitData.Direction, unitData.PlayerId);
            }

            Notebook.NoteData($"Unit at {attackerCoordinate} attacked target at {targetCoordinate}");
        }
        
        public async UniTask ExecuteMove(Vector2Int unitCoordinate, Vector2Int targetCoordinate)
        {
            var unit = _visual.GetUnitAtCoordinate(unitCoordinate);

            if (unit == null)
            {
                Notebook.NoteError("Move failed: No unit found at coordinate");
                return;
            }

            // Get the unit data
            var unitData = GetUnitData(unitCoordinate);
            if (unitData == null)
            {
                Notebook.NoteError("Move failed: No unit data found");
                return;
            }

            // Calculate path using A* pathfinding
            var path = HexPathfinder.CalculatePath(Grid, unitCoordinate, targetCoordinate);

            if (!path.IsValid)
            {
                Notebook.NoteWarning($"No valid path found from {unitCoordinate} to {targetCoordinate}");
                return;
            }

            // Execute movement along the path
            await ExecutePathMovement(unit, unitData, path);
        }

        private async UniTask ExecutePathMovement(BaseBattleUnit unit, BattleUnitData unitData, TravelPath path)
        {
            if (path.TotalSteps == 0)
            {
                // Already at destination, just update ownership indicators
                GridSelection.UpdateHexOwnershipIndicators();
                Notebook.NoteData($"Unit is already at destination {path.EndCoordinate}");
                return;
            }

            // Clear ownership from starting position
            var startHexOperator = Grid.GetHexOperatorAtCoordinate(path.StartCoordinate);
            if (startHexOperator != null)
            {
                startHexOperator.SetHasPlayerUnit(false);
                startHexOperator.SetHasBotUnit(false);
            }

            // Execute movement step by step
            await ExecutePathStep(unit, unitData, path, 0);
        }

        private async UniTask ExecutePathStep(BaseBattleUnit unit, BattleUnitData unitData, TravelPath path, int stepIndex)
        {
            if (stepIndex >= path.TotalSteps)
            {
                // Movement complete
                unitData.Coordinate = path.EndCoordinate;

                // Update GridSelection coordinate if this unit was selected
                if (GridSelection.IsCoordinateSelected(path.StartCoordinate))
                {
                    GridSelection.UpdateSelectedCoordinate(path.EndCoordinate);
                }

                // Update hex ownership indicators
                GridSelection.UpdateHexOwnershipIndicators();

                Notebook.NoteData($"Unit completed path movement to {path.EndCoordinate}");
                return;
            }

            var step = path.Steps[stepIndex];
            var targetWorldPosition = Grid.GetWorldPosition(step.Coordinate);

            // Execute rotation (in parallel with move)
            unit.Rotate(unitData.Coordinate, step.Coordinate, unitData.Direction)
                .ContinueWith(newDirection =>
                {
                    unitData.Direction = newDirection;
                })
                .Forget();
                

            // Execute move to this step
            await unit.Move(targetWorldPosition);
            
            // Update current position
            unitData.Coordinate = step.Coordinate;

            // Update visual coordinate tracking
            if (stepIndex == 0)
            {
                _visual.UpdateUnitCoordinate(path.StartCoordinate, step.Coordinate);
            }
            else
            {
                _visual.UpdateUnitCoordinate(path.Steps[stepIndex - 1].Coordinate, step.Coordinate);
            }

            // Continue to next step
            await ExecutePathStep(unit, unitData, path, stepIndex + 1);
        }
        
        public async UniTask ExecuteRotate(Vector2Int unitCoordinate, Vector2Int targetCoordinate)
        {
            var unit = _visual.GetUnitAtCoordinate(unitCoordinate);

            // Get the unit data
            var unitData = GetUnitData(unitCoordinate);

            // Execute the rotation - rotator will calculate and return the target direction
            var newDirection = await unit.Rotate(unitCoordinate, targetCoordinate, unitData.Direction);
            
            // Update the unit data direction after rotation is complete
            unitData.Direction = newDirection;
        }

        public bool SpawnUnitAtCoordinate(string unitId, Vector2Int coordinate)
        {
            // Validate that the coordinate is valid for spawning
            if (!Grid.IsValidForAbility(AbilityMode.Spawn, coordinate))
            {
                Notebook.NoteWarning($"Cannot spawn unit at {coordinate} - invalid location");
                return false;
            }

            // Check if a unit already exists at this coordinate
            if (GetUnitData(coordinate) != null)
            {
                Notebook.NoteWarning($"Cannot spawn unit at {coordinate} - location occupied");
                return false;
            }

            // Get unit config
            var unitConfig = _config.GetBattleUnit(unitId);

            // Get player ID
            var playerId = PlayerAccount.PlayerId;
            if (string.IsNullOrEmpty(playerId))
            {
                Notebook.NoteError("Cannot spawn unit - no player logged in");
                return false;
            }

            // Create unit data
            var unitData = new BattleUnitData()
            {
                BattleUnitId = unitId,
                Coordinate = coordinate,
                Direction = HexDirection.North, // Default direction
                Health = unitConfig.MaxHealth,
                Level = 1, // Starting level
                IsDead = false,
                PlayerId = playerId
            };

            // Add to record
            Record.BattleUnits.Add(unitData);

            // Spawn visually
            _visual.SpawnUnit(unitData);

            // Update hex ownership indicators
            GridSelection.UpdateHexOwnershipIndicators();

            Notebook.NoteData($"Spawned {unitId} for player {playerId} at {coordinate}");
            return true;
        }
        
        private void DealDamageToTarget(Vector2Int attackerCoordinate, HexDirection facingDirection, string attackerPlayerId)
        {
            // Calculate the target coordinate in the facing direction
            var targetCoordinate = GridUtils.NextHex(attackerCoordinate, facingDirection);

            // Check if there's a unit at the target coordinate
            var targetUnitData = GetUnitData(targetCoordinate);
            if (targetUnitData == null)
            {
                Notebook.NoteData($"No unit found at {targetCoordinate} to attack");
                return;
            }

            // Check if the target is an enemy (different player ID)
            if (targetUnitData.PlayerId == attackerPlayerId)
            {
                Notebook.NoteData($"Cannot attack own unit at {targetCoordinate}");
                return;
            }

            // Get attacker config to determine damage
            var attackerConfig = GetUnitConfig(GetUnitData(attackerCoordinate)?.BattleUnitId);
            if (attackerConfig == null)
            {
                Notebook.NoteError("Cannot find attacker config for damage calculation");
                return;
            }

            // Calculate damage (for now, basic calculation)
            int damage = attackerConfig.AttackPower;

            // Apply damage to target
            targetUnitData.Health -= damage;
            targetUnitData.Health = Mathf.Max(0, targetUnitData.Health); // Ensure health doesn't go below 0

            // Trigger hit animation on target unit
            var targetUnit = _visual.GetUnitAtCoordinate(targetCoordinate);
            targetUnit?.GetHit();

            // Update health bar
            UpdateUnitHealthBar(targetCoordinate, targetUnitData.Health, targetUnitData.IsDead);

            Notebook.NoteData($"Unit at {attackerCoordinate} dealt {damage} damage to enemy at {targetCoordinate}. Enemy health: {targetUnitData.Health}");

            // Check if target is dead
            if (targetUnitData.Health <= 0 && !targetUnitData.IsDead)
            {
                targetUnitData.IsDead = true;
                HandleUnitDeath(targetCoordinate);
                Notebook.NoteData($"Unit at {targetCoordinate} has died");
            }
        }

        private void UpdateUnitHealthBar(Vector2Int unitCoordinate, int currentHealth, bool isDead)
        {
            var unit = _visual.GetUnitAtCoordinate(unitCoordinate);
            if (unit != null)
            {
                // Get the unit config to know max health
                var unitData = GetUnitData(unitCoordinate);
                var unitConfig = GetUnitConfig(unitData?.BattleUnitId);
                if (unitConfig != null)
                {
                    float healthPercentage = isDead ? 0f : (float)currentHealth / unitConfig.MaxHealth;
                    unit.UpdateHealthBar(healthPercentage);
                }
            }
        }

        private void HandleUnitDeath(Vector2Int unitCoordinate)
        {
            var unit = _visual.GetUnitAtCoordinate(unitCoordinate);
            if (unit != null)
            {
                unit.SetIsDead(true);
                // Note: Unit despawning will be handled by the turn system or battle end
            }
        }

        private void ClearUnitSelection()
        {
            var allUnits = _visual.GetSpawnedUnits();
            foreach (var unit in allUnits)
            {
                unit.SetGlow(false);
            }
        }
    }
}