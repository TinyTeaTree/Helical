using UnityEngine;

public interface ISoundConfig
{
    public AudioClip Clip { get; }
    public AudioType Type { get; }
    public string Id { get; }
    public float Volume { get; }
    public float Pitch { get; }
}