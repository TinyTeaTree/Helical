using UnityEngine;

namespace Game.Utils
{
    /// <summary>
    /// Utility component for controlling the ColorGlow shader on a mesh.
    /// Attach this to any GameObject with a MeshRenderer to easily control glow effects.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class GlowEffect : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Color glowColor = Color.white;
        [SerializeField] private float glowIntensity = 1.0f;
        [SerializeField] private bool updateInRealtime = true;

        private Material glowMaterial;
        private static readonly int GlowColorProperty = Shader.PropertyToID("_GlowColor");
        private static readonly int GlowIntensityProperty = Shader.PropertyToID("_GlowIntensity");

        private void Awake()
        {
            if (meshRenderer != null && meshRenderer.material != null)
            {
                // Create instance of material to avoid modifying the shared material
                glowMaterial = meshRenderer.material;
                UpdateGlowProperties();
            }
        }

        private void Update()
        {
            if (updateInRealtime && glowMaterial != null)
            {
                UpdateGlowProperties();
            }
        }

        /// <summary>
        /// Sets the glow color and updates the material.
        /// </summary>
        public void SetGlowColor(Color color)
        {
            glowColor = color;
            UpdateGlowProperties();
        }

        /// <summary>
        /// Sets the glow intensity and updates the material.
        /// </summary>
        public void SetGlowIntensity(float intensity)
        {
            glowIntensity = Mathf.Max(0, intensity);
            UpdateGlowProperties();
        }

        /// <summary>
        /// Sets both color and intensity at once.
        /// </summary>
        public void SetGlow(Color color, float intensity)
        {
            glowColor = color;
            glowIntensity = Mathf.Max(0, intensity);
            UpdateGlowProperties();
        }

        /// <summary>
        /// Enables or disables the glow effect by setting intensity to 0 or restoring the previous value.
        /// </summary>
        public void SetGlowEnabled(bool enabled)
        {
            if (glowMaterial != null)
            {
                glowMaterial.SetFloat(GlowIntensityProperty, enabled ? glowIntensity : 0);
            }
        }

        private void UpdateGlowProperties()
        {
            if (glowMaterial != null)
            {
                glowMaterial.SetColor(GlowColorProperty, glowColor);
                glowMaterial.SetFloat(GlowIntensityProperty, glowIntensity);
            }
        }

        private void OnDestroy()
        {
            // Clean up the material instance
            if (glowMaterial != null)
            {
                Destroy(glowMaterial);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Update in editor when values change
            if (Application.isPlaying && glowMaterial != null)
            {
                UpdateGlowProperties();
            }
        }
#endif
    }
}

