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
    [SerializeField] private Transform _gridRoot;
    [SerializeField] private Camera _sceneCamera;

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
        if (_grid == null)
        {
            Debug.LogWarning("MapMaker: GridSO reference is missing.", this);
            return;
        }

        if (_resourcePack == null)
        {
            Debug.LogWarning("MapMaker: GridResourcePack reference is missing.", this);
            return;
        }

        var parent = _gridRoot != null ? _gridRoot : transform;
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Populate Grid");

        RestoreCameraState();

        ClearGridInternal(parent);

        var gridData = new GridData(_grid.Width, _grid.Height, _grid.Id);

        foreach (var hex in _grid.Cells)
        {
            gridData.SetCell(hex);
        }

        for (int x = 0; x < gridData.Width; x++)
        {
            var rowObject = new GameObject($"Row_{x}");
            Undo.RegisterCreatedObjectUndo(rowObject, "Create Grid Row");
            rowObject.transform.SetParent(parent);
            rowObject.transform.localPosition = Vector3.zero;

            bool rowHasChildren = false;

            for (int y = 0; y < gridData.Height; y++)
            {
                var cell = gridData.GetCell(x, y);
                if (cell.Type == HexType.None)
                {
                    continue;
                }

                var prefab = _resourcePack.GetHex(cell.Type);
                if (prefab == null)
                {
                    Debug.LogWarning($"MapMaker: Missing prefab for HexType {cell.Type}.", this);
                    continue;
                }

                var instance = PrefabUtility.InstantiatePrefab(prefab.gameObject, rowObject.transform) as GameObject;
                if (instance == null)
                {
                    continue;
                }

                instance.name = $"Hex_({x},{y})";

                var worldXZ = cell.Coordinate.ToWorldXZ();
                instance.transform.localPosition = new Vector3(worldXZ.x, 0f, worldXZ.y);
                instance.transform.localScale = Vector3.one * GridUtils.HexScaleModifier;

                var hexOperator = instance.GetComponent<HexOperator>();
                if (hexOperator != null)
                {
                    hexOperator.Initialize(cell.Coordinate);
                }

                rowHasChildren = true;
            }

            if (!rowHasChildren)
            {
                Undo.DestroyObjectImmediate(rowObject);
            }
        }

        if (_grid.GluePrefab != null)
        {
            var glue = PrefabUtility.InstantiatePrefab(_grid.GluePrefab, null) as GameObject;
            if (glue != null)
            {
                Undo.RegisterCreatedObjectUndo(glue, "Create Glue Object");
                glue.name = $"{_grid.GluePrefab.name}_Glue";
                glue.transform.localPosition = Vector3.zero;
                _glueInstance = glue;
            }
        }
        else
        {
            _glueInstance = null;
        }

        if (_grid.CameraAnchorPrefab != null)
        {
            var anchor = PrefabUtility.InstantiatePrefab(_grid.CameraAnchorPrefab, null) as GameObject;
            if (anchor != null)
            {
                anchor.transform.position = _grid.CameraAnchorPrefab.transform.position;
                anchor.transform.rotation = _grid.CameraAnchorPrefab.transform.rotation;
                Undo.RegisterCreatedObjectUndo(anchor, "Create Camera Anchor");
                anchor.name = $"{_grid.CameraAnchorPrefab.name}_Anchor";
                _cameraAnchorInstance = anchor;
                AlignCameraToAnchor(anchor.transform);
            }
        }
        else
        {
            _cameraAnchorInstance = null;
        }
    }

    public void ClearLevel()
    {
        var parent = _gridRoot != null ? _gridRoot : transform;
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Clear Grid");
        RestoreCameraState();
        DestroyImmediate(_glueInstance);
        DestroyImmediate(_cameraAnchorInstance);
        ClearGridInternal(parent);
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

    private Camera GetTargetCamera()
    {
        if (_sceneCamera != null)
        {
            return _sceneCamera;
        }

        return Camera.main;
    }

    private void AlignCameraToAnchor(Transform anchorTransform)
    {
        var camera = GetTargetCamera();
        if (camera == null)
        {
            Debug.LogWarning("MapMaker: No camera available to align with anchor.", this);
            return;
        }

        StoreCameraState(camera.transform);

        Undo.SetTransformParent(camera.transform, anchorTransform, "Parent Camera to Anchor");
        camera.transform.localPosition = Vector3.zero;
        camera.transform.localRotation = Quaternion.identity;
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

        var camera = GetTargetCamera();
        if (camera == null)
        {
            return;
        }

        Undo.SetTransformParent(camera.transform, _originalCameraParent, "Restore Camera Parent");
        camera.transform.localPosition = _originalCameraLocalPosition;
        camera.transform.localRotation = _originalCameraLocalRotation;
        _cameraStateStored = false;
    }
#endif
}
