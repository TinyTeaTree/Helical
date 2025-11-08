using Core;
using Services;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CastleVisual : BaseVisual<CastleFeature>
    {
        private Dictionary<Vector2Int, CastleOperator> _castleCache = new Dictionary<Vector2Int, CastleOperator>();

        public void SpawnCastle(CastleData castleData)
        {
            var worldPosition = Feature.Grid.GetWorldPosition(castleData.Coordinate);
            var rotation = castleData.Direction.ToRotation();

            var prefab = Feature.CastleAssetPack.GetCastle(castleData.CastleType);
            if (prefab == null)
            {
                Debug.LogWarning($"Castle prefab not found for type: {castleData.CastleType}");
                return;
            }

            var castleInstance = Summoner.CreateAsset(prefab, transform);
            castleInstance.transform.localPosition = worldPosition;
            castleInstance.transform.localRotation = rotation;

            var castleOperator = castleInstance.GetComponent<CastleOperator>();
            castleOperator.Initialize(castleData.Coordinate);

            // Track castle by coordinate
            _castleCache[castleData.Coordinate] = castleOperator;
        }

        public void DespawnAllCastles()
        {
            foreach (var castle in _castleCache.Values)
            {
                Destroy(castle.gameObject);
            }
            _castleCache.Clear();
        }

        public CastleOperator GetCastleAtCoordinate(Vector2Int coordinate)
        {
            _castleCache.TryGetValue(coordinate, out var castle);
            return castle;
        }

        public IReadOnlyDictionary<Vector2Int, CastleOperator> GetCastleCache()
        {
            return _castleCache;
        }
    }
}