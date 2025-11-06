using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class BattleUnits : BaseVisualFeature<BattleUnitsVisual>, IBattleUnits, IAppLaunchAgent, IBattleLaunchAgent
    {
        [Inject] public BattleUnitsRecord Record { get; set; }
        [Inject] public ILocalConfigService ConfigService { get; set; }
        [Inject] public IGrid Grid { get; set; }
        [Inject] public IGridSelection GridSelection { get; set; }

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
            
            TMP_PopulateTestSkeletons();
            
            return UniTask.CompletedTask;
        }
        
        // TODO: Remove this temporary function - for testing only
        private void TMP_PopulateTestSkeletons()
        {
            var playerId = _bootstrap.Features.Get<IPlayerAccount>().PlayerId;
            var random = new System.Random();
            var directions = System.Enum.GetValues(typeof(HexDirection));
            
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

                if (!Grid.IsValidHex(randomCoordinate))
                {
                    continue;
                }   
                
                // Random direction
                var randomDirection = (HexDirection)directions.GetValue(random.Next(directions.Length));
                
                Record.BattleUnits.Add(new BattleUnitData()
                {
                    BattleUnitId = "Skeleton",
                    Coordinate = randomCoordinate,
                    Direction = randomDirection,
                    Health = 40,
                    IsDead = false,
                    PlayerId = playerId
                });
            }
            
            Notebook.NoteData($"TMP: Spawned {Record.BattleUnits.Count} test skeletons");
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
                
                Notebook.NoteData($"Unit moved from {unitCoordinate} to {targetCoordinate}");
            });
        }
        
        public void ExecuteRotate(Vector2Int unitCoordinate, Vector2Int targetCoordinate)
        {
            var unit = _visual.GetUnitAtCoordinate(unitCoordinate);
            
            if (unit == null)
            {
                Notebook.NoteError("Rotate failed: No unit found at coordinate");
                return;
            }
            
            // Get the unit data
            var unitData = GetUnitData(unitCoordinate);
            if (unitData == null)
            {
                Notebook.NoteError("Rotate failed: No unit data found");
                return;
            }
            
            // Execute the rotation - rotator will calculate and return the target direction
            unit.Rotate(unitCoordinate, targetCoordinate, unitData.Direction, (newDirection) =>
            {
                // Update the unit data direction after rotation is complete
                unitData.Direction = newDirection;
                
                Notebook.NoteData($"Unit rotated to {newDirection}");
            });
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