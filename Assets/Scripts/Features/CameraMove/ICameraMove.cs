using Core;
using UnityEngine;

namespace Game
{
    public interface ICameraMove : IFeature
    {
        void InitializeBounds();
        void Start();
        void Halt();
        void LerpToCoordinate(Vector2Int coordinate);
    }
}