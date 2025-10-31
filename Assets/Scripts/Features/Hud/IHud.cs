using Core;
using UnityEngine;

namespace Game
{
    public interface IHud : IFeature
    {
        bool IsReady { get; }
        Camera HudCamera { get; }
        Transform HudRoot { get; }
        void SetCanvas(Canvas visualCanvas);
    }
}