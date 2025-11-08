using System;
using System.Collections.Generic;
using Services;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class CastlePrefabEntry
    {
        public string CastleType;
        public CastleOperator Prefab;
    }

    [CreateAssetMenu(fileName = "Castle Asset Pack", menuName = "Game/Asset Packs/Castle Asset Pack")]
    public class CastleAssetPack : BaseAssetPack
    {
        [SerializeField]
        private List<CastlePrefabEntry> _castleEntries = new List<CastlePrefabEntry>();

        private Dictionary<string, CastleOperator> _castleLookup;

        private void BuildLookups()
        {
            _castleLookup = new Dictionary<string, CastleOperator>();
            foreach (var entry in _castleEntries)
            {
                _castleLookup[entry.CastleType] = entry.Prefab;
            }
        }

        public CastleOperator GetCastle(string castleType)
        {
            if (_castleLookup == null)
            {
                BuildLookups();
            }

            if (_castleLookup.TryGetValue(castleType, out var prefab))
            {
                return prefab;
            }

            return null;
        }

        public List<string> GetAvailableCastleTypes()
        {
            var types = new List<string>();
            foreach (var entry in _castleEntries)
            {
                if (entry.Prefab != null && !types.Contains(entry.CastleType))
                {
                    types.Add(entry.CastleType);
                }
            }

            return types;
        }
    }
}
