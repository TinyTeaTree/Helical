using UnityEngine;


[System.Serializable]
public class SoundInstruction
{
    [SerializeField] public AudioClip Clip;
    [SerializeField] public AudioType Type;
    [SerializeField, Range(0f, 1f)] public float Volume = 1f;
    [SerializeField, Range(0f, 2f)] public float Pitch = 1f;
}
