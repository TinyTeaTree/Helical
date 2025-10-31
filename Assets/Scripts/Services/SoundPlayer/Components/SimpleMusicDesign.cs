using UnityEngine;

namespace Services
{
    public class SimpleMusicDesign : BaseSoundDesign
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private float _fadeIn = 0.25f;
        [SerializeField] private float _fadeOut = 0.25f;

        public override SoundType Type => SoundType.Music;
        public override AudioClip Clip => _clip;
        public override float Volume => 1f;
        public override float Pitch => 1f;
        public override bool Loop => true;
        
        public override float FadeIn => _fadeIn;
        public override float FadeOut => _fadeOut;
    }
}