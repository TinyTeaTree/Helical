using Core;
using UnityEngine;

namespace Game
{
    public interface IBattleUnits : IFeature
    {
        void SpawnAllUnits();
        void UpdateUnitSelection(Vector2Int? coordinate);
        BattleUnitData GetUnitData(Vector2Int coordinate);
        void ExecuteAttack(Vector2Int attackerCoordinate, Vector2Int targetCoordinate);
    }
}