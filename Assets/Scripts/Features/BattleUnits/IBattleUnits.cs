using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface IBattleUnits : IFeature
    {
        void SpawnAllUnits();
        void UpdateUnitSelection(Vector2Int? coordinate);
        BattleUnitData GetUnitData(Vector2Int coordinate);
        BattleUnitConfig GetUnitConfig(string unitId);
        UniTask ExecuteAttack(Vector2Int attackerCoordinate, Vector2Int targetCoordinate);
        UniTask ExecuteMove(Vector2Int unitCoordinate, Vector2Int targetCoordinate);
        UniTask ExecuteRotate(Vector2Int unitCoordinate, Vector2Int targetCoordinate);
        bool SpawnUnitAtCoordinate(string unitId, Vector2Int spawnCoordinate);
    }
}