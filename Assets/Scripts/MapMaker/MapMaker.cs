using Game;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MapMaker : MonoBehaviour
{
    [SerializeField] private GridSO _grid;
    [SerializeField] private GridResourcePack _resourcePack;
    [SerializeField] private CastleAssetPack _castleAssetPack;
    [SerializeField] private BattleUnitsAssetPack _battleUnitsAssetPack;
    [SerializeField] private Transform _gridRoot;
    [SerializeField] private Camera _sceneCamera;
    [SerializeField] private HexOperator _hexNone;

    public CastleAssetPack CastleAssetPack => _castleAssetPack;

#if UNITY_EDITOR
    public GridSO Grid => _grid;
    private GameObject _glueInstance;
    private GameObject _cameraAnchorInstance;
    private Transform _originalCameraParent;
    private Vector3 _originalCameraLocalPosition;
    private Quaternion _originalCameraLocalRotation;
    private bool _cameraStateStored;

    public void PopulateLevel()
    {
        var parent = _gridRoot;
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Populate Grid");

        RestoreCameraState();

        ClearGridInternal(parent);
        
        var gridData = _grid.GetData();

        for (int x = 0; x < gridData.Width; x++)
        {
            var rowObject = new GameObject($"Row_{x}");
            Undo.RegisterCreatedObjectUndo(rowObject, "Create Grid Row");
            rowObject.transform.SetParent(parent);
            rowObject.transform.localPosition = Vector3.zero;

            bool rowHasChildren = false;

            for (int y = 0; y < gridData.Height; y++)
            {
                var coordinate = new Vector2Int(x, y);
                var cell = gridData.GetCell(x, y);

                CreateHexInstance(cell.Type, coordinate, rowObject.transform, false);
                rowHasChildren = true;
            }

            if (!rowHasChildren)
            {
                Undo.DestroyObjectImmediate(rowObject);
            }
        }

        // Spawn castles
        if (gridData.Castles != null)
        {
            foreach (var castleData in gridData.Castles)
            {
                CreateCastleInstance(castleData, parent);
            }
        }

        // Spawn predetermined units
        if (gridData.PredeterminedUnits != null)
        {
            foreach (var unitData in gridData.PredeterminedUnits)
            {
                CreateUnitInstance(unitData, parent);
            }
        }

        var glue = PrefabUtility.InstantiatePrefab(_grid.GluePrefab, null) as GameObject;
        Undo.RegisterCreatedObjectUndo(glue, "Create Glue Object");
        glue.name = $"{_grid.GluePrefab.name}_Glue";
        glue.transform.localPosition = Vector3.zero;
        _glueInstance = glue;

        var anchor = PrefabUtility.InstantiatePrefab(_grid.CameraAnchorPrefab, null) as GameObject;
        anchor.transform.position = _grid.CameraAnchorPrefab.transform.position;
        anchor.transform.rotation = _grid.CameraAnchorPrefab.transform.rotation;
        Undo.RegisterCreatedObjectUndo(anchor, "Create Camera Anchor");
        anchor.name = $"{_grid.CameraAnchorPrefab.name}_Anchor";
        _cameraAnchorInstance = anchor;
        AlignCameraToAnchor(anchor.transform);
    }

    public void ClearLevel()
    {
        var parent = _gridRoot;
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Clear Grid");
        RestoreCameraState();
        DestroyImmediate(_glueInstance);
        DestroyImmediate(_cameraAnchorInstance);
        ClearGridInternal(parent);
    }

    public void UpdateHexType(Vector2Int coordinate, HexType newType, Transform hexTransform)
    {
        Undo.RegisterCompleteObjectUndo(_grid, "Change Hex Type");
        UpdateGridCellData(coordinate, newType);

        var parent = hexTransform.parent;
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Change Hex Type");

        var localPosition = hexTransform.localPosition;
        var localRotation = hexTransform.localRotation;
        var localScale = hexTransform.localScale;

        CreateHexInstance(newType, coordinate, parent, true, localPosition, localRotation, localScale);

        Undo.DestroyObjectImmediate(hexTransform.gameObject);

        MarkGridDirty();
    }

    private void ClearGridInternal(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i);
            Undo.DestroyObjectImmediate(child.gameObject);
        }
        _glueInstance = null;
        _cameraAnchorInstance = null;
    }

    private GameObject CreateHexInstance(HexType type, Vector2Int coordinate, Transform parent, bool registerUndo, Vector3? localPosition = null, Quaternion? localRotation = null, Vector3? localScale = null)
    {
        var prefab = type == HexType.None ? _hexNone : _resourcePack.GetHex(type);
        var instance = PrefabUtility.InstantiatePrefab(prefab.gameObject, parent) as GameObject;

        if (registerUndo)
        {
            Undo.RegisterCreatedObjectUndo(instance, "Create Hex");
        }

        instance.name = $"Hex_({coordinate.x},{coordinate.y})";

        if (localPosition.HasValue)
        {
            instance.transform.localPosition = localPosition.Value;
            instance.transform.localRotation = localRotation ?? Quaternion.identity;
            instance.transform.localScale = localScale ?? Vector3.one;
        }
        else
        {
            var worldXZ = coordinate.ToWorldXZ();
            instance.transform.localPosition = new Vector3(worldXZ.x, 0f, worldXZ.y);
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one * GridUtils.HexScaleModifier;
        }

        var hexOperator = instance.GetComponent<HexOperator>();
        AttachEditorHexOperator(instance, type);

        return instance;
    }

    public void CreateCastleInstance(CastleData castleData, Transform parent)
    {
        var worldPosition = castleData.Coordinate.ToWorldXZ();
        var rotation = castleData.Direction.ToRotation();

        var prefab = _castleAssetPack.GetCastle(castleData.CastleType);
        if (prefab == null)
        {
            Debug.LogWarning($"Castle prefab not found for type: {castleData.CastleType}");
            return;
        }

        var instance = PrefabUtility.InstantiatePrefab(prefab.gameObject, parent) as GameObject;
        Undo.RegisterCreatedObjectUndo(instance, "Create Castle");

        instance.name = $"Castle_{castleData.CastleType}_{castleData.Coordinate.x}_{castleData.Coordinate.y}";
        instance.transform.localPosition = new Vector3(worldPosition.x, 0f, worldPosition.y);
        instance.transform.localRotation = rotation;

        var castleOperator = instance.GetComponent<CastleOperator>();
        castleOperator.Initialize(castleData.Coordinate);
        castleOperator.SetNormalState();
    }

    private void CreateUnitInstance(PredeterminedUnitData unitData, Transform parent)
    {
        var worldPosition = unitData.Coordinate.ToWorldXZ();

        var prefab = _battleUnitsAssetPack.GetUnitPrefab(unitData.UnitId);
        if (prefab == null)
        {
            Debug.LogWarning($"Unit prefab not found for type: {unitData.UnitId}");
            return;
        }

        var instance = PrefabUtility.InstantiatePrefab(prefab.gameObject, parent) as GameObject;
        Undo.RegisterCreatedObjectUndo(instance, "Create Unit");

        instance.name = $"Unit_{unitData.UnitId}_{unitData.Coordinate.x}_{unitData.Coordinate.y}";
        instance.transform.localPosition = new Vector3(worldPosition.x, 0f, worldPosition.y);
        instance.transform.localRotation = Quaternion.identity;

        var baseUnit = instance.GetComponent<BaseBattleUnit>();
    }

    private void AttachEditorHexOperator(GameObject instance, HexType hexType)
    {
        var editorOperator = instance.GetComponent<EditorHexOperator>();
        if (editorOperator == null)
        {
            editorOperator = instance.AddComponent<EditorHexOperator>();
        }

        editorOperator.Initialize(this, hexType);
    }

    private void AlignCameraToAnchor(Transform anchorTransform)
    {
        StoreCameraState(_sceneCamera.transform);

        Undo.SetTransformParent(_sceneCamera.transform, anchorTransform, "Parent Camera to Anchor");
        _sceneCamera.transform.localPosition = Vector3.zero;
        _sceneCamera.transform.localRotation = Quaternion.identity;
    }

    private void StoreCameraState(Transform cameraTransform)
    {
        if (_cameraStateStored)
        {
            return;
        }

        _originalCameraParent = cameraTransform.parent;
        _originalCameraLocalPosition = cameraTransform.localPosition;
        _originalCameraLocalRotation = cameraTransform.localRotation;
        _cameraStateStored = true;
    }

    private void RestoreCameraState()
    {
        if (!_cameraStateStored)
        {
            return;
        }

        Undo.SetTransformParent(_sceneCamera.transform, _originalCameraParent, "Restore Camera Parent");
        _sceneCamera.transform.localPosition = _originalCameraLocalPosition;
        _sceneCamera.transform.localRotation = _originalCameraLocalRotation;
        _cameraStateStored = false;
    }

    private void UpdateGridCellData(Vector2Int coordinate, HexType newType)
    {
        var cells = _grid.Cells;
        var index = -1;
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].Coordinate == coordinate)
            {
                index = i;
                break;
            }
        }

        var updated = new HexData
        {
            Coordinate = coordinate,
            Type = newType
        };

        if (index >= 0)
        {
            cells[index] = updated;
        }
        else
        {
            //cells.Add(updated); //TODO: Fixe me
        }
    }

    private void MarkGridDirty()
    {
        EditorUtility.SetDirty(_grid);
        AssetDatabase.SaveAssets();
    }

    public void PlaceUnitAtHex(Vector2Int coordinate, string unitId, int level, string playerId)
    {
        Undo.RegisterCompleteObjectUndo(_grid, "Place Unit");
        _grid.AddPredeterminedUnit(unitId, coordinate, level, playerId);
        MarkGridDirty();
    }

    public void RemoveUnitFromHex(Vector2Int coordinate)
    {
        Undo.RegisterCompleteObjectUndo(_grid, "Remove Unit");
        _grid.RemovePredeterminedUnit(coordinate);
        MarkGridDirty();
    }

    public bool HasUnitAtHex(Vector2Int coordinate)
    {
        return _grid.GetPredeterminedUnitAt(coordinate).HasValue;
    }

    public string GetUnitInfoAtHex(Vector2Int coordinate)
    {
        var unitData = _grid.GetPredeterminedUnitAt(coordinate);
        if (unitData.HasValue)
        {
            return $"{unitData.Value.UnitId} (Lv.{unitData.Value.Level}) - {unitData.Value.PlayerId}";
        }
        return "No Unit";
    }
#endif
}
