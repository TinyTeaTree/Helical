using System.Collections.Generic;
using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class BattleUnitsVisual : BaseVisual<BattleUnits>
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
            battleUnit.Initialize(unitData.BattleUnitId);

            _spawnedUnits.Add(battleUnit);

            return battleUnit;
        }

        public void DespawnUnit(BaseBattleUnit battleUnit)
        {
            if (_spawnedUnits.Remove(battleUnit))
            {
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
    }
}