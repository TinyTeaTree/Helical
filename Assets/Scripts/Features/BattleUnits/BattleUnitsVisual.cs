using System.Collections.Generic;
using System.Linq;
using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class BattleUnitsVisual : BaseVisual<BattleUnits>
    {
        private Dictionary<Vector2Int, BaseBattleUnit> _unitsByCoordinate = new Dictionary<Vector2Int, BaseBattleUnit>();

        public BaseBattleUnit SpawnUnit(BattleUnitData unitData)
        {
            var worldPosition = Feature.Grid.GetWorldPosition(unitData.Coordinate);
            var rotation = unitData.Direction.ToRotation();
            
            var prefab = Feature.AssetPack.GetUnitPrefab(unitData.BattleUnitId);
            var unitInstance = Summoner.CreateAsset(prefab, transform);
            unitInstance.transform.localPosition = worldPosition;
            unitInstance.transform.localRotation = rotation;

            var battleUnit = unitInstance.GetComponent<BaseBattleUnit>();
            battleUnit.Initialize(unitData.BattleUnitId);

            // Track unit by coordinate (one unit per coordinate)
            _unitsByCoordinate[unitData.Coordinate] = battleUnit;

            return battleUnit;
        }

        public void DespawnUnit(BaseBattleUnit battleUnit)
        {
            // Find and remove from coordinate tracking
            var coordinateToRemove = _unitsByCoordinate.FirstOrDefault(kvp => kvp.Value == battleUnit).Key;
            if (_unitsByCoordinate.ContainsKey(coordinateToRemove))
            {
                _unitsByCoordinate.Remove(coordinateToRemove);
                Destroy(battleUnit.gameObject);
            }
        }

        public void DespawnAllUnits()
        {
            foreach (var unit in _unitsByCoordinate.Values)
            {
                Destroy(unit.gameObject);
            }
            _unitsByCoordinate.Clear();
        }

        public IReadOnlyList<BaseBattleUnit> GetSpawnedUnits()
        {
            return _unitsByCoordinate.Values.ToList();
        }
        
        public BaseBattleUnit GetUnitAtCoordinate(Vector2Int coordinate)
        {
            _unitsByCoordinate.TryGetValue(coordinate, out var unit);
            return unit;
        }
        
        /// <summary>
        /// Updates the coordinate tracking when a unit moves from one coordinate to another
        /// </summary>
        public void UpdateUnitCoordinate(Vector2Int oldCoordinate, Vector2Int newCoordinate)
        {
            if (_unitsByCoordinate.TryGetValue(oldCoordinate, out var unit))
            {
                _unitsByCoordinate.Remove(oldCoordinate);
                _unitsByCoordinate[newCoordinate] = unit;
            }
        }
    }
}