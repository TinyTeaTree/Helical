using UnityEngine;

public class RandomClipSound : RegularSound, ISoundConfig
{
    public AudioClip[] _randomClips;

    public override AudioClip Clip => _randomClips.GetRandom();
}
