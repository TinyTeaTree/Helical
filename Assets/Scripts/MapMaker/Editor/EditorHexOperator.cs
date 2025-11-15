#if UNITY_EDITOR
using Game;
using UnityEngine;

[ExecuteInEditMode]
public class EditorHexOperator : MonoBehaviour
{
    [SerializeField] private HexType _selectedType;
    [SerializeField] private string _selectedUnitId = "";
    [SerializeField] private int _selectedUnitLevel = 1;
    [SerializeField] private string _selectedPlayerId = "Bot";
    [SerializeField] private HexDirection _selectedUnitDirection = HexDirection.South;

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

    public void PlaceUnit()
    {
        if (!string.IsNullOrEmpty(_selectedUnitId))
        {
            _mapMaker.PlaceUnitAtHex(Coordinate, _selectedUnitId, _selectedUnitLevel, _selectedPlayerId, _selectedUnitDirection);
        }
    }

    public void RemoveUnit()
    {
        _mapMaker.RemoveUnitFromHex(Coordinate);
    }

    public bool HasUnit()
    {
        return _mapMaker.HasUnitAtHex(Coordinate);
    }

    public string GetUnitInfo()
    {
        return _mapMaker.GetUnitInfoAtHex(Coordinate);
    }
}
#endif

