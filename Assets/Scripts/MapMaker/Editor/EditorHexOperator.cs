#if UNITY_EDITOR
using Game;
using UnityEngine;

[ExecuteInEditMode]
public class EditorHexOperator : MonoBehaviour
{
    [SerializeField] private HexType _selectedType;

    private MapMaker _mapMaker;
    private HexOperator _hexOperator;

    public HexType SelectedType
    {
        get => _selectedType;
        set => _selectedType = value;
    }

    public Vector2Int Coordinate => _hexOperator.Coordinate;

    public void Initialize(MapMaker mapMaker, HexType initialType)
    {
        _mapMaker = mapMaker;
        _hexOperator = GetComponent<HexOperator>();
        _selectedType = initialType;
    }

    public void ApplySelectedType()
    {
        _mapMaker.UpdateHexType(Coordinate, _selectedType, transform);
    }
}
#endif

