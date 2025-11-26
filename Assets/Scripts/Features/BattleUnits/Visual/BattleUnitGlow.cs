using UnityEngine;

namespace Game
{
    /// <summary>
    /// Component that manages glow effect for battle units by adding/removing a glow material.
    /// </summary>
    public class BattleUnitGlow : MonoBehaviour
    {
        [SerializeField] private Material _glowMaterial;
        [SerializeField] private Renderer _renderer;
        
        private Material[] _originalMaterials;
        private bool _isGlowing;

        private void Awake()
        {
            if (_renderer != null)
            {
                _originalMaterials = _renderer.materials;
            }
        }

        public void SetGlow(bool isGlowing)
        {
            if (_isGlowing == isGlowing || _renderer == null || _glowMaterial == null)
                return;

            _isGlowing = isGlowing;

            if (_isGlowing)
            {
                AddGlowMaterial();
            }
            else
            {
                RemoveGlowMaterial();
            }
        }

        private void AddGlowMaterial()
        {
            // Create a new array with one additional slot for the glow material
            Material[] newMaterials = new Material[_originalMaterials.Length + 1];
            
            // Copy original materials
            for (int i = 0; i < _originalMaterials.Length; i++)
            {
                newMaterials[i] = _originalMaterials[i];
            }
            
            // Add glow material at the end
            newMaterials[_originalMaterials.Length] = _glowMaterial;
            
            _renderer.materials = newMaterials;
        }

        private void RemoveGlowMaterial()
        {
            // Restore original materials
            _renderer.materials = _originalMaterials;
        }
    }
}

