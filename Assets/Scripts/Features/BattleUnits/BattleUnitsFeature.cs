using System;
using System.Collections.Generic;
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

                // Initialize ability cooldowns
                InitializeAbilityCooldowns(battleUnitData, unitConfig);

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

            // Check if there's a living unit at the coordinate
            var unitData = GetUnitData(coordinate.Value);
            if (unitData == null || unitData.IsDead)
            {
                return; // No unit or dead unit at coordinate
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
        
        public async UniTask ExecuteAttack(Vector2Int attackerCoordinate, Vector2Int targetCoordinate, int actionPoints, int interceptionPoint)
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
                    .Rotate(attackerCoordinate, targetCoordinate, unitData.Direction, 0.3f) // Default duration for manual rotation
                    .ContinueWith(newDirection =>
                    {
                        unitData.Direction = newDirection;
                    })
                    .Forget();
            }

            // Calculate duration from action points using central constant
            float duration = actionPoints * (TurnFeature.SECONDS_PER_100_ACTION_POINTS / 100f);

            // Calculate interception time (when damage actually occurs)
            float interceptionTime = interceptionPoint * (TurnFeature.SECONDS_PER_100_ACTION_POINTS / 100f);

            // Execute attack with calculated duration and interception timing
            await attackerUnit.Attack(duration, interceptionTime, () =>
            {
                // This callback is executed at the interception point
                if (unitData != null)
                {
                    DealDamageToTarget(attackerCoordinate, unitData.Direction, unitData.PlayerId);
                }
            });

            Notebook.NoteData($"Unit at {attackerCoordinate} attacked target at {targetCoordinate}");
        }

        public async UniTask ExecuteCleaveAttack(Vector2Int attackerCoordinate, Vector2Int targetCoordinate, int actionPoints, int interceptionPoint)
        {
            var attackerUnit = _visual.GetUnitAtCoordinate(attackerCoordinate);

            if (attackerUnit == null)
            {
                Notebook.NoteError("Cleave attack failed: No attacker unit found");
                return;
            }

            // Get the unit data and config
            var unitData = GetUnitData(attackerCoordinate);
            var unitConfig = GetUnitConfig(unitData?.BattleUnitId);
            if (unitConfig == null || unitData == null)
            {
                Notebook.NoteError("Cleave attack failed: Cannot find unit config or data");
                return;
            }

            // Find the cleave attack action config
            var cleaveAction = unitConfig.Actions.Find(a => a.Ability == AbilityMode.CleaveAttack);
            if (cleaveAction == null)
            {
                Notebook.NoteError("Cleave attack failed: No cleave attack action found in config");
                return;
            }

            // Execute rotation towards the target (in parallel with attack)
            attackerUnit
                .Rotate(attackerCoordinate, targetCoordinate, unitData.Direction, 0.3f)
                .ContinueWith(newDirection =>
                {
                    unitData.Direction = newDirection;
                })
                .Forget();

            // Calculate duration from action points using central constant
            float duration = actionPoints * (TurnFeature.SECONDS_PER_100_ACTION_POINTS / 100f);

            // Calculate interception time (when damage actually occurs)
            float interceptionTime = interceptionPoint * (TurnFeature.SECONDS_PER_100_ACTION_POINTS / 100f);

            // Execute attack with calculated duration and interception timing
            await attackerUnit.Attack(duration, interceptionTime, () =>
            {
                // This callback is executed at the interception point
                DealDamageToCleaveTargets(attackerCoordinate, targetCoordinate, unitData.Direction, unitData.PlayerId, cleaveAction);
            });

            Notebook.NoteData($"Unit at {attackerCoordinate} cleave attacked target at {targetCoordinate}");
        }

        private void DealDamageToCleaveTargets(Vector2Int attackerCoordinate, Vector2Int primaryTargetCoordinate, HexDirection unitDirection, string attackerPlayerId, BattleUnitConfig.Action cleaveAction)
        {
            var targetCoordinates = new List<Vector2Int>();

            // Always include the primary target
            targetCoordinates.Add(primaryTargetCoordinate);

            // Calculate additional cleave targets based on configuration
            foreach (var cleaveTarget in cleaveAction.CleaveTargets)
            {
                Vector2Int currentCoord = primaryTargetCoordinate;

                // Follow the sequence of directions from the primary target
                foreach (var direction in cleaveTarget.Directions)
                {
                    // Transpose the direction based on unit's facing direction
                    var transposedDirection = GridUtils.TransposeDirection(direction, unitDirection);
                    currentCoord = GridUtils.NextHex(currentCoord, transposedDirection);
                }

                // Add the final coordinate if it's not already in the list
                if (!targetCoordinates.Contains(currentCoord))
                {
                    targetCoordinates.Add(currentCoord);
                }
            }

            // Deal damage to all calculated targets
            foreach (var targetCoord in targetCoordinates)
            {
                DealDamageToTargetAtCoordinate(attackerCoordinate, targetCoord, attackerPlayerId);
            }

            if (targetCoordinates.Count > 1)
            {
                Notebook.NoteData($"Cleave attack hit {targetCoordinates.Count} targets");
            }
        }


        public async UniTask ExecuteMove(Vector2Int unitCoordinate, Vector2Int targetCoordinate, int actionPoints, int interceptionPoint)
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

            // Calculate duration from action points using central constant
            float duration = actionPoints * (TurnFeature.SECONDS_PER_100_ACTION_POINTS / 100f);

            // Calculate interception time (when unit logically arrives at new position)
            float interceptionTime = interceptionPoint * (TurnFeature.SECONDS_PER_100_ACTION_POINTS / 100f);

            // Execute single step movement with interception timing
            await ExecuteSingleStepMovement(unit, unitData, targetCoordinate, duration, interceptionTime);
        }

        private async UniTask ExecuteSingleStepMovement(BaseBattleUnit unit, BattleUnitData unitData, Vector2Int targetCoordinate, float duration, float interceptionTime)
        {
            // Store the old coordinate for visual tracking
            var oldCoordinate = unitData.Coordinate;

            // Execute rotation (in parallel with move)
            unit.Rotate(unitData.Coordinate, targetCoordinate, unitData.Direction, 0.3f) // Default rotation duration
                .ContinueWith(newDirection =>
                {
                    unitData.Direction = newDirection;
                })
                .Forget();

            // Get target world position
            var targetWorldPosition = Grid.GetWorldPosition(targetCoordinate);

            // Start movement animation
            var moveTask = unit.Move(targetWorldPosition, duration);

            // Wait for interception time and update logical position
            if (interceptionTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(interceptionTime));

                // Update unit logical position at interception point
                unitData.Coordinate = targetCoordinate;


                // Update hex ownership indicators
                GridSelection.UpdateHexOwnershipIndicators();

                Notebook.NoteData($"Unit logically arrived at {targetCoordinate} at interception time {interceptionTime}s");
            }

            // Wait for the remaining movement animation to complete
            await moveTask;

            // If interception hasn't happened yet, update position now
            if (unitData.Coordinate != targetCoordinate)
            {
                unitData.Coordinate = targetCoordinate;
                GridSelection.UpdateHexOwnershipIndicators();
            }

            Notebook.NoteData($"Unit movement animation completed for {targetCoordinate} in {duration} seconds");
        }
        
        public async UniTask ExecuteRotate(Vector2Int unitCoordinate, Vector2Int targetCoordinate, int actionPoints)
        {
            var unit = _visual.GetUnitAtCoordinate(unitCoordinate);

            // Get the unit data
            var unitData = GetUnitData(unitCoordinate);

            // Calculate duration from action points using central constant
            float duration = actionPoints * (TurnFeature.SECONDS_PER_100_ACTION_POINTS / 100f);

            // Execute the rotation with calculated duration
            var newDirection = await unit.Rotate(unitCoordinate, targetCoordinate, unitData.Direction, duration);

            // Update the unit data direction after rotation is complete
            unitData.Direction = newDirection;
        }

        public async UniTask ExecuteWait(Vector2Int unitCoordinate, int actionPoints, int interceptionPoint)
        {
            // Calculate duration from action points using central constant
            float duration = actionPoints * (TurnFeature.SECONDS_PER_100_ACTION_POINTS / 100f);

            // Simply wait for the calculated duration - no visual changes or logical updates needed
            await UniTask.Delay(TimeSpan.FromSeconds(duration));

            Notebook.NoteData($"Unit at {unitCoordinate} waited for {duration} seconds");
        }

        public async UniTask ExecuteRangeAttack(Vector2Int attackerCoordinate, Vector2Int orderedTargetCoordinate, int actionPoints, int interceptionPoint)
        {
            var attackerUnit = _visual.GetUnitAtCoordinate(attackerCoordinate);

            if (attackerUnit == null)
            {
                Notebook.NoteError("Range attack failed: No attacker unit found");
                return;
            }

            // Get the unit data and config
            var unitData = GetUnitData(attackerCoordinate);
            var unitConfig = GetUnitConfig(unitData?.BattleUnitId);
            if (unitConfig == null || unitData == null)
            {
                Notebook.NoteError("Range attack failed: Cannot find unit config or data");
                return;
            }

            // Find the range attack action config
            var rangeAttackAction = unitConfig.Actions.Find(a => a.Ability == AbilityMode.RangeAttack);
            if (rangeAttackAction == null)
            {
                Notebook.NoteError("Range attack failed: No RangeAttack action found in config");
                return;
            }

            int attackRange = rangeAttackAction.Range;

            // Find the closest valid target within range of the ordered target coordinate
            var actualTargetCoordinate = FindClosestValidTarget(attackerCoordinate, orderedTargetCoordinate, attackRange);

            if (actualTargetCoordinate == null)
            {
                Notebook.NoteData($"No valid target found within range {attackRange} of {orderedTargetCoordinate}");
                return;
            }

            // Calculate projectile travel time based on distance and speed from config
            Vector3 attackerWorldPos = GridUtils.ToWorldX0Z(attackerCoordinate);
            Vector3 targetWorldPos = GridUtils.ToWorldX0Z(actualTargetCoordinate.Value);
            float distance = Vector3.Distance(attackerWorldPos, targetWorldPos);
            float projectileTravelTime = distance / rangeAttackAction.ProjectileSpeed;

            // Execute rotation towards the actual target (in parallel with attack)
            attackerUnit
                .Rotate(attackerCoordinate, actualTargetCoordinate.Value, unitData.Direction, 0.3f)
                .ContinueWith(newDirection =>
                {
                    unitData.Direction = newDirection;
                })
                .Forget();

            // Calculate duration from action points using central constant
            float duration = actionPoints * (TurnFeature.SECONDS_PER_100_ACTION_POINTS / 100f);

            // Calculate interception time (for attack animation timing)
            float interceptionTime = interceptionPoint * (TurnFeature.SECONDS_PER_100_ACTION_POINTS / 100f);
            
            // Execute attack animation with interception timing (visual only)
            await attackerUnit.Attack(duration, interceptionTime, () =>
            {
                // Execute particle shooting with calculated travel time
                attackerUnit.Shoot(attackerCoordinate, actualTargetCoordinate.Value, projectileTravelTime).Forget();

                // Schedule damage to occur when projectile arrives (after travel time)
                UniTask.Delay(System.TimeSpan.FromSeconds(projectileTravelTime)).ContinueWith(() =>
                {
                    DealDamageToTargetAtCoordinate(attackerCoordinate, actualTargetCoordinate.Value, unitData.PlayerId);
                }).Forget();
            });

            Notebook.NoteData($"Unit at {attackerCoordinate} range attacked target at {actualTargetCoordinate.Value} (ordered: {orderedTargetCoordinate})");
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

            // Initialize ability cooldowns
            InitializeAbilityCooldowns(unitData, unitConfig);

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
            if (targetUnitData.Health > 0)
            {
                targetUnit?.GetHit();
            }

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
            // Get the unit data
            var unitData = GetUnitData(unitCoordinate);
            if (unitData == null || !unitData.IsDead)
            {
                return; // Unit not found or not dead
            }

            // Get the visual unit
            var unit = _visual.GetUnitAtCoordinate(unitCoordinate);
            if (unit != null)
            {
                // Trigger death animation
                unit.SetIsDead(true);

                // Remove health bar widget from HUD
                Hud.DestroyWidget(unit.HealthBarAnchor);
            }

            // Remove unit from the battle record
            Record.BattleUnits.Remove(unitData);

            // Update hex ownership indicators to remove dead unit's ownership
            GridSelection.UpdateHexOwnershipIndicators();

            Notebook.NoteData($"Unit {unitData.BattleUnitId} at {unitCoordinate} has been removed from battle");
        }

        private Vector2Int? FindClosestValidTarget(Vector2Int attackerCoordinate, Vector2Int orderedTargetCoordinate, int range)
        {
            // Check if the ordered target coordinate is within range of the attacker
            int distanceToOrderedTarget = GridUtils.HexDistance(attackerCoordinate, orderedTargetCoordinate);

            if (distanceToOrderedTarget <= range)
            {
                return orderedTargetCoordinate;
            }
            else
            {
                // Ordered target is out of range, find the closest hex within range of attacker to the ordered target
                Vector2Int closestValidHex = FindClosestHexInRange(attackerCoordinate, orderedTargetCoordinate, range);
                
                return closestValidHex;
            }
        }

        private Vector2Int FindClosestHexInRange(Vector2Int attackerCoordinate, Vector2Int targetCoordinate, int range)
        {
            // Get all coordinates within range of the attacker
            var coordinatesInRange = GridUtils.GetCoordinatesInRange(attackerCoordinate, range);

            Vector2Int closestHex = attackerCoordinate;
            int closestDistance = int.MaxValue;

            foreach (var coord in coordinatesInRange)
            {
                // Calculate distance from target coordinate to this valid hex
                int distance = GridUtils.HexDistance(targetCoordinate, coord);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHex = coord;
                }
            }

            return closestHex;
        }

        private void DealDamageToTargetAtCoordinate(Vector2Int attackerCoordinate, Vector2Int targetCoordinate, string attackerPlayerId)
        {
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

        private void InitializeAbilityCooldowns(BattleUnitData unitData, BattleUnitConfig unitConfig)
        {
            unitData.AbilityCooldowns.Clear();

            foreach (var action in unitConfig.Actions)
            {
                var cooldownData = new BattleUnitData.AbilityCooldownData
                {
                    Ability = action.Ability,
                    UsedThisTurn = 0,
                    TurnsToCooldown = 0
                };

                unitData.AbilityCooldowns.Add(cooldownData);
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