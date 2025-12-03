using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class CursorTexturePair
    {
        [SerializeField] private AbilityMode _abilityMode;
        [SerializeField] private Texture2D _texture;
        [SerializeField] private Vector2 _hotspot = Vector2.zero;

        public AbilityMode AbilityMode => _abilityMode;
        public Texture2D Texture => _texture;
        public Vector2 Hotspot => _hotspot;
    }

    public struct CursorData
    {
        public Texture2D Texture;
        public Vector2 Hotspot;

        public CursorData(Texture2D texture, Vector2 hotspot)
        {
            Texture = texture;
            Hotspot = hotspot;
        }
    }

    [System.Serializable]
    public class CursorConfig : BaseConfig
    {
        [SerializeField]
        private List<CursorTexturePair> _cursorTextures = new List<CursorTexturePair>();

        public List<CursorTexturePair> CursorTextures => _cursorTextures;

        public CursorData? GetCursorData(AbilityMode mode)
        {
            var pair = _cursorTextures.Find(p => p.AbilityMode == mode);
            if (pair != null)
            {
                return new CursorData(pair.Texture, pair.Hotspot);
            }
            return null;
        }

        public Texture2D GetCursorTexture(AbilityMode mode)
        {
            var pair = _cursorTextures.Find(p => p.AbilityMode == mode);
            return pair?.Texture;
        }
    }
}