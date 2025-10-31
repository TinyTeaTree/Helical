using UnityEngine;
using UnityEngine.Serialization;

public class IndexClipSound : RegularSound, ISoundConfig
{
    public AudioClip[] _indexedClips;
    public int Index { get; set; }

    public override AudioClip Clip => _indexedClips[Index % _indexedClips.Length];
}