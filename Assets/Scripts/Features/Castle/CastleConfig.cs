using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class CastleUnitPurchaseEntry
    {
        [SerializeField]
        private string _unitId;

        [SerializeField]
        private int _goldCost;

        public string UnitId => _unitId;
        public int GoldCost => _goldCost;
    }

    [Serializable]
    public class CastleConfigEntry
    {
        [SerializeField]
        private string _castleType;

        [SerializeField]
        private string _displayName;

        [SerializeField]
        private List<CastleUnitPurchaseEntry> _availableUnits = new List<CastleUnitPurchaseEntry>();

        public string CastleType => _castleType;
        public string DisplayName => _displayName;
        public List<CastleUnitPurchaseEntry> AvailableUnits => _availableUnits;
    }

    [System.Serializable]
    public class CastleConfig : BaseConfig
    {
        [SerializeField]
        private List<CastleConfigEntry> _castleConfigs = new List<CastleConfigEntry>();

        public List<CastleConfigEntry> CastleConfigs => _castleConfigs;

        public CastleConfigEntry GetCastleConfig(string castleType)
        {
            return _castleConfigs.Find(config => config.CastleType == castleType);
        }

        public List<CastleUnitPurchaseEntry> GetAvailableUnits(string castleType)
        {
            var config = GetCastleConfig(castleType);
            return config != null ? config.AvailableUnits : new List<CastleUnitPurchaseEntry>();
        }

        public int GetUnitCost(string castleType, string unitId)
        {
            var config = GetCastleConfig(castleType);
            if (config == null)
            {
                return -1; // Invalid castle type
            }

            var unitEntry = config.AvailableUnits.Find(unit => unit.UnitId == unitId);
            return unitEntry != null ? unitEntry.GoldCost : -1; // Unit not available or cost not found
        }
    }
}
