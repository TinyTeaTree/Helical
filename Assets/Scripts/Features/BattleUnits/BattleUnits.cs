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
            
            attackerUnit.Attack();
            
            Notebook.NoteData($"Unit at {attackerCoordinate} attacked target at {targetCoordinate}");
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