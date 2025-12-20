using System;
using System.Collections.Generic;
using Services;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class FloatingTextPresetEntry
    {
        public FloatingTextPresetType PresetType;
        public FloatingTextPresetSO Preset;
        public FloatingTextWidget Prefab;
    }

    [CreateAssetMenu(fileName = "Floating Text Asset Pack", menuName = "Game/Asset Packs/Floating Text Asset Pack")]
    public class FloatingTextAssetPack : BaseAssetPack
    {
        [SerializeField]
        private List<FloatingTextPresetEntry> _presetEntries = new List<FloatingTextPresetEntry>();

        private Dictionary<FloatingTextPresetType, FloatingTextPresetSO> _presetLookup;
        private Dictionary<FloatingTextPresetType, FloatingTextWidget> _prefabLookup;

        private void OnEnable()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            _presetLookup = new Dictionary<FloatingTextPresetType, FloatingTextPresetSO>();
            _prefabLookup = new Dictionary<FloatingTextPresetType, FloatingTextWidget>();

            foreach (var entry in _presetEntries)
            {
                if (entry.Preset != null)
                {
                    _presetLookup[entry.PresetType] = entry.Preset;
                }
                if (entry.Prefab != null)
                {
                    _prefabLookup[entry.PresetType] = entry.Prefab;
                }
            }
        }

        public FloatingTextPresetSO GetPreset(FloatingTextPresetType presetType)
        {
            if (_presetLookup == null)
            {
                BuildLookup();
            }

            if (_presetLookup.TryGetValue(presetType, out var preset))
            {
                return preset;
            }

            return null;
        }

        public FloatingTextWidget GetPrefab(FloatingTextPresetType presetType)
        {
            if (_prefabLookup == null)
            {
                BuildLookup();
            }

            if (_prefabLookup.TryGetValue(presetType, out var prefab))
            {
                return prefab;
            }

            return null;
        }

        public List<FloatingTextPresetType> GetAvailablePresetTypes()
        {
            var types = new List<FloatingTextPresetType>();
            foreach (var entry in _presetEntries)
            {
                if (entry.Preset != null && !types.Contains(entry.PresetType))
                {
                    types.Add(entry.PresetType);
                }
            }
            return types;
        }
    }
}
