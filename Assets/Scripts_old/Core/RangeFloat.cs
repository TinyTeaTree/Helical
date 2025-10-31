using UnityEngine;

[System.Serializable]
public struct RangeFloat
{
    [SerializeField] float _from;
    [SerializeField] float _to;

    public float From => _from;
    public float To => _to;
    public float Length => _to - _from;

    public RangeFloat(float from, float to)
    {
        _from = from;
        _to = to;
    }

    public float GetRandom()
    {
        return Random.Range(_from, _to);
    }
}