using UnityEngine;

namespace Services
{
    public interface IDesignSound
    {
        SoundType Type { get; }
        
        AudioClip Clip { get; }
        float Volume { get; }
        float Pitch { get; }
        
        bool Loop { get; }
        
        float FadeIn { get; }
        float FadeOut { get; }
        
    }
}