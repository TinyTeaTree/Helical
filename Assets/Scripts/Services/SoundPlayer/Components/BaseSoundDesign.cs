using Core;
using UnityEngine;

namespace Services
{
    public abstract class BaseSoundDesign : BaseSO, IDesignSound
    {
        public abstract SoundType Type { get; }
        public abstract AudioClip Clip { get; }
        public abstract float Volume { get; }
        public abstract float Pitch { get; }
        public abstract bool Loop { get; }
        
        public abstract float FadeIn { get; }
        public abstract float FadeOut { get; }
    }
}