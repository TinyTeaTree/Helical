using System;
using System.Collections.Generic;
using Services;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class HexPrefabEntry
    {
        public HexType Type;
        public HexOperator Prefab;
    }

    [CreateAssetMenu(fileName = "Grid Resource Pack", menuName = "Game/Resource Packs/Grid Resource Pack")]
    public class GridResourcePack : BaseAssetPack
    {
        [SerializeField]
        private List<HexPrefabEntry> _hexEntries = new List<HexPrefabEntry>();

        [SerializeField]
        private List<GridSO> _grids = new List<GridSO>();

        private Dictionary<HexType, HexOperator> _hexLookup;
        private Dictionary<string, GridSO> _gridLookup;

        private void BuildLookups()
        {
            _hexLookup = new Dictionary<HexType, HexOperator>();
            foreach (var entry in _hexEntries)
            {
                _hexLookup[entry.Type] = entry.Prefab;
            }

            _gridLookup = new Dictionary<string, GridSO>();
            foreach (var grid in _grids)
            {
                if (grid != null && !string.IsNullOrEmpty(grid.Id))
                {
                    _gridLookup[grid.Id] = grid;
                }
            }
        }

        public HexOperator GetHex(HexType type)
        {
            if (_hexLookup == null)
            {
                BuildLookups();
            }

            if (_hexLookup.TryGetValue(type, out var prefab))
            {
                return prefab;
            }

            return null;
        }

        public GridSO GetGrid(string id)
        {
            if (_gridLookup == null)
            {
                BuildLookups();
            }

            if (_gridLookup.TryGetValue(id, out var grid))
            {
                return grid;
            }

            return null;
        }

        public List<HexType> GetAvailableHexTypes()
        {
            var types = new List<HexType>();
            foreach (var entry in _hexEntries)
            {
                if (entry.Prefab != null && !types.Contains(entry.Type))
                {
                    types.Add(entry.Type);
                }
            }
            
            // Always include None type
            if (!types.Contains(HexType.None))
            {
                types.Add(HexType.None);
            }
            
            return types;
        }
    }
}

