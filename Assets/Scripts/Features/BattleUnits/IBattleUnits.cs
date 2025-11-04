using Core;
using UnityEngine;

namespace Game
{
    public interface IBattleUnits : IFeature
    {
        void UpdateUnitSelectionAtCoordinate(Vector2Int? coordinate);
        BattleUnitData GetUnitDataAtCoordinate(Vector2Int coordinate);
        void ExecuteAttack(Vector2Int attackerCoordinate, Vector2Int targetCoordinate);
    }
}