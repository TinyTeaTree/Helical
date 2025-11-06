using Core;
using UnityEngine;

namespace Game
{
    public interface IGridSelection : IFeature
    {
        void Start();
        void Halt();
        void SetAbilityMode(AbilityMode mode);
        void UpdateSelectedCoordinate(Vector2Int newCoordinate);
        bool IsCoordinateSelected(Vector2Int coordinate);
    }
}