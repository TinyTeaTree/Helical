using UnityEngine;
using Random = UnityEngine.Random;

namespace Services
{
    public class SoundDesign : BaseSoundDesign
    {
        [SerializeField] public Vector2 PitchRange;

        [SerializeField] private AudioClip _clip;

        [SerializeField] private SoundType _type;

        [SerializeField] private bool _loop;

        [SerializeField] private float _volume = 1f;

        [SerializeField] private float _fadeIn = 0f;
        [SerializeField] private float _fadeOut = 0f;

        public override SoundType Type => _type;
        public override AudioClip Clip => _clip;
        public override float Volume => _volume;
        public override float Pitch => PitchRange.x == PitchRange.y ? (PitchRange.x == 0 ? 1f : PitchRange.x) : Random.Range(PitchRange.x, PitchRange.y);
        public override bool Loop => _loop;

        public override float FadeIn => _fadeIn;
        public override float FadeOut => _fadeOut;
    }
}