using UnityEngine;

namespace Services
{
    public class SimpleEffectDesign : BaseSoundDesign
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private float _volume = 1f;

        public override SoundType Type => SoundType.Effect;
        public override AudioClip Clip => _clip;
        public override float Volume => _volume;
        public override float Pitch => 1f;
        public override bool Loop => false;
        public override float FadeIn => 0f;
        public override float FadeOut => 0f;
    }
}