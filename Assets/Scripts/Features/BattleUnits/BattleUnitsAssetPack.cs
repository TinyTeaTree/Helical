using System;
using System.Collections.Generic;
using Services;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class BattleUnitPrefabEntry
    {
        public string UnitId;
        public GameObject Prefab;
        public Sprite Photo;
    }

    [CreateAssetMenu(fileName = "Battle Units Asset Pack", menuName = "Game/Asset Packs/Battle Units Asset Pack")]
    public class BattleUnitsAssetPack : BaseAssetPack
    {
        [SerializeField]
        private List<BattleUnitPrefabEntry> _unitEntries = new List<BattleUnitPrefabEntry>();

        private Dictionary<string, GameObject> _unitLookup;

        private void OnEnable()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            _unitLookup = new Dictionary<string, GameObject>();
            foreach (var entry in _unitEntries)
            {
                if (!string.IsNullOrEmpty(entry.UnitId) && entry.Prefab != null)
                {
                    _unitLookup[entry.UnitId] = entry.Prefab;
                }
            }
        }

        public GameObject GetUnitPrefab(string unitId)
        {
            if (_unitLookup == null)
            {
                BuildLookup();
            }

            if (_unitLookup.TryGetValue(unitId, out var prefab))
            {
                return prefab;
            }

            return null;
        }

        public List<string> GetAvailableUnitIds()
        {
            var ids = new List<string>();
            foreach (var entry in _unitEntries)
            {
                if (!string.IsNullOrEmpty(entry.UnitId) && entry.Prefab != null && !ids.Contains(entry.UnitId))
                {
                    ids.Add(entry.UnitId);
                }
            }
            return ids;
        }
    }
}

