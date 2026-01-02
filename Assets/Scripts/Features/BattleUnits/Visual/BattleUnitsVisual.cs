using System.Collections.Generic;
using System.Linq;
using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class BattleUnitsVisual : BaseVisual<BattleUnitsFeature>
    {
        private List<BaseBattleUnit> _spawnedUnits = new List<BaseBattleUnit>();
        

        public BaseBattleUnit SpawnUnit(BattleUnitData unitData)
        {
            var worldPosition = Feature.Grid.GetWorldPosition(unitData.Coordinate);
            var rotation = unitData.Direction.ToRotation();

            var prefab = Feature.AssetPack.GetUnitPrefab(unitData.BattleUnitId);
            var unitInstance = Summoner.CreateAsset(prefab, transform);
            unitInstance.transform.localPosition = worldPosition;
            unitInstance.transform.localRotation = rotation;

            var battleUnit = unitInstance.GetComponent<BaseBattleUnit>();
            battleUnit.SetFeature(Feature);
            battleUnit.Initialize(unitData.BattleUnitId);

            // Cache the instance GUID on the visual for future reference
            battleUnit.InstanceGuid = unitData.InstanceGuid;

            // Create health bar widget and store reference in the unit
            var healthBarWidget = Feature.Hud.CreateWidget(Feature.AssetPack.HealthBarPrefab, battleUnit.HealthBarAnchor);
            battleUnit.SetHealthBarWidget(healthBarWidget as BattleUnitHealthBar);

            // Initialize health bar to full health
            battleUnit.UpdateHealthBar(1.0f);

            // Add to spawned units list
            _spawnedUnits.Add(battleUnit);

            return battleUnit;
        }

        public void DespawnUnit(BaseBattleUnit battleUnit)
        {
            if (_spawnedUnits.Contains(battleUnit))
            {
                if (battleUnit.HealthBarWidget != null)
                {
                    Feature.Hud.DestroyWidget(battleUnit.HealthBarWidget);
                }
                _spawnedUnits.Remove(battleUnit);
                Destroy(battleUnit.gameObject);
            }
        }

        public void DespawnAllUnits()
        {
            foreach (var unit in _spawnedUnits)
            {
                Destroy(unit.gameObject);
            }
            _spawnedUnits.Clear();
        }

        public IReadOnlyList<BaseBattleUnit> GetSpawnedUnits()
        {
            return _spawnedUnits;
        }
        
        public BaseBattleUnit GetUnitAtCoordinate(Vector2Int coordinate)
        {
            // Find the unit data at this coordinate
            var unitData = Feature.GetUnitData(coordinate);
            if (unitData == null)
                return null;

            // Find the visual unit with matching GUID
            return _spawnedUnits.Find(unit => unit.InstanceGuid == unitData.InstanceGuid);
        }
        
    }
}