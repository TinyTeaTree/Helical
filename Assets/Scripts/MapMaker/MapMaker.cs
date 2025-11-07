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

#if UNITY_EDITOR
    public GridSO Grid => _grid;
    private GameObject _glueInstance;

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

        ClearGridInternal(parent);

        var gridData = new GridData(_grid.Width, _grid.Height);

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


        var glue = PrefabUtility.InstantiatePrefab(_grid.GluePrefab, null) as GameObject;

        Undo.RegisterCreatedObjectUndo(glue, "Create Glue Object");
        glue.name = $"{_grid.GluePrefab.name}_Glue";
        glue.transform.localPosition = Vector3.zero;
        _glueInstance = glue;
    }

    public void ClearLevel()
    {
        var parent = _gridRoot != null ? _gridRoot : transform;
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Clear Grid");
        ClearGridInternal(parent);
        DestroyImmediate(_glueInstance);
        _glueInstance = null;
    }

    private void ClearGridInternal(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i);
            Undo.DestroyObjectImmediate(child.gameObject);
        }
        _glueInstance = null;
    }
#endif
}
