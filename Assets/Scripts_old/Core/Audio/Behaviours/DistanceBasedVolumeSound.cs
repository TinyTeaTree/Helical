using System.Collections;
using UnityEngine;


public class DistanceBasedVolumeSound : WagBehaviour, ISoundConfig
{
    [SerializeField] AudioClip _clip;
    [SerializeField] Transform _distance;

    public AudioClip Clip => _clip;

    public AudioType Type => AudioType.Effects;

    public string Id => "Doesnt matter";

    public float Volume
    {
        get
        {
            var distance = (transform.position - _distance.position).magnitude;

            return Mathf.Lerp(1, 0, distance / 10f);
        }
    }

    public float Pitch => 1f;
}
