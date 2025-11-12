using Core;
using UnityEngine;

namespace Game
{
    public interface ICastleGUI : IFeature
    {
        void ShowCastleSelection(Vector2Int coordinate);
        void HideCastleSelection();
    }
}