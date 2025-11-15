using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface IGrid : IFeature, IHexProvider
    {

        UniTask LoadGrid(string gridId);
        Vector3 GetWorldPosition(Vector2Int coordinate);
        bool IsValidForAbility(AbilityMode ability, Vector2Int coordinate);
        HexOperator GetHexOperatorAtCoordinate(Vector2Int coordinate);
        void GetCameraAnchor(out Vector3 position, out Quaternion rotation);
    }
}