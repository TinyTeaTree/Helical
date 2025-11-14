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
            
            TMP_PopulateTestBattleUnits();
            
            return UniTask.CompletedTask;
        }
        
        // TODO: Remove this temporary function - for testing only
        private void TMP_PopulateTestBattleUnits()
        {
            var playerId = _bootstrap.Features.Get<IPlayerAccount>().PlayerId;
            var random = new System.Random();
            var directions = System.Enum.GetValues(typeof(HexDirection));
            
            // Get available unit IDs from the asset pack
            var availableUnitIds = _assetPack.GetAvailableUnitIds();
            if (availableUnitIds.Count == 0)
            {
                Notebook.NoteError("No units available in asset pack!");
                return;
            }
            
            int unitsToSpawn = 16;
            int attempts = 0;
            int maxAttempts = 100; // Prevent infinite loop
            
            while (Record.BattleUnits.Count < unitsToSpawn && attempts < maxAttempts)
            {
                attempts++;
                
                // Generate random coordinate (spread across the map)
                var randomCoordinate = new Vector2Int(
                    random.Next(5, 25),  // x between 5 and 25
                    random.Next(5, 25)   // y between 5 and 25
                );
                
                // Check if a unit already exists at this coordinate
                if (Record.BattleUnits.Exists(u => u.Coordinate == randomCoordinate))
                {
                    continue;
                }

                if (!Grid.IsValidForAbility(AbilityMode.Spawn, randomCoordinate))
                {
                    continue;
                }   
                
                // Randomly select a unit type from available units
                var randomUnitId = availableUnitIds[random.Next(availableUnitIds.Count)];
                
                // Get unit config to get proper health value
                var unitConfig = _config.GetBattleUnit(randomUnitId);
                int unitHealth = unitConfig != null ? unitConfig.MaxHealth : 40; // Fallback to 40 if config not found
                
                // Random direction
                var randomDirection = (HexDirection)directions.GetValue(random.Next(directions.Length));
                
                Record.BattleUnits.Add(new BattleUnitData()
                {
                    BattleUnitId = randomUnitId,
                    Coordinate = randomCoordinate,
                    Direction = randomDirection,
                    Health = unitHealth,
                    Level = random.Next(1, 6), // Random level between 1 and 5
                    IsDead = false,
                    PlayerId = playerId
                });
            }

            // Update hex ownership indicators after spawning all units
            GridSelection.UpdateHexOwnershipIndicators();

            Notebook.NoteData($"TMP: Spawned {Record.BattleUnits.Count} randomized battle units");
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
        
        public void ExecuteAttack(Vector2Int attackerCoordinate, Vector2Int targetCoordinate)
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
                attackerUnit.Rotate(attackerCoordinate, targetCoordinate, unitData.Direction, (newDirection) =>
                {
                    // Update direction after rotation completes
                    unitData.Direction = newDirection;
                });
            }
            
            // Execute attack (in parallel with rotation)
            attackerUnit.Attack();
            
            Notebook.NoteData($"Unit at {attackerCoordinate} attacked target at {targetCoordinate}");
        }
        
        public void ExecuteMove(Vector2Int unitCoordinate, Vector2Int targetCoordinate)
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
            
            // Execute rotation (in parallel with move)
            // The rotator will calculate the target direction and return it
            unit.Rotate(unitCoordinate, targetCoordinate, unitData.Direction, (newDirection) =>
            {
                // Update direction after rotation completes
                unitData.Direction = newDirection;
            });
            
            // Execute move (in parallel with rotation)
            var targetWorldPosition = Grid.GetWorldPosition(targetCoordinate);
            unit.Move(targetWorldPosition, () =>
            {
                // Update the unit coordinate in the record after movement is complete
                unitData.Coordinate = targetCoordinate;
                
                // Update the visual's coordinate tracking
                _visual.UpdateUnitCoordinate(unitCoordinate, targetCoordinate);
                
                // Update the GridSelection coordinate to track the unit's new position
                // Only update if this unit was the one selected
                if (GridSelection.IsCoordinateSelected(unitCoordinate))
                {
                    GridSelection.UpdateSelectedCoordinate(targetCoordinate);
                }

                // Update hex ownership indicators after movement
                GridSelection.UpdateHexOwnershipIndicators();

                Notebook.NoteData($"Unit moved from {unitCoordinate} to {targetCoordinate}");
            });
        }
        
        public void ExecuteRotate(Vector2Int unitCoordinate, Vector2Int targetCoordinate)
        {
            var unit = _visual.GetUnitAtCoordinate(unitCoordinate);

            // Get the unit data
            var unitData = GetUnitData(unitCoordinate);

            // Execute the rotation - rotator will calculate and return the target direction
            unit.Rotate(unitCoordinate, targetCoordinate, unitData.Direction, (newDirection) =>
            {
                // Update the unit data direction after rotation is complete
                unitData.Direction = newDirection;

                Notebook.NoteData($"Unit rotated to {newDirection}");
            });
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