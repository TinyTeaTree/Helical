using Core;
using UnityEngine;

namespace Game
{
    public interface IFloatingText : IFeature
    {
        void ShowDamageText(string damageText, Transform anchor, FloatingTextPresetSO preset);
    }
}
