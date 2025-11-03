using Core;
using UnityEngine;

namespace Game
{
    public interface IBattleUnits : IFeature
    {
        void UpdateUnitSelectionAtCoordinate(Vector2Int? coordinate);
    }
}