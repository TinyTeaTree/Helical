using UnityEngine;

public class PitchRangeSound : RegularSound, ISoundConfig
{
    [SerializeField] float _pitchRange = 0.2f;

    public override float Pitch => UnityEngine.Random.Range(base.Pitch - _pitchRange, base.Pitch + _pitchRange);
}
