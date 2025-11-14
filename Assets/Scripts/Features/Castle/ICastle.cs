using Core;
using System.Collections.Generic;

namespace Game
{
    public interface ICastle : IFeature
    {
        CastleConfig Config { get; }
        List<CastleUnitPurchaseEntry> GetAvailableUnits(string castleType);
        int GetUnitCost(string castleType, string unitId);
    }
}