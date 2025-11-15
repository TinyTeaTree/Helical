#if UNITY_EDITOR
using Game;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorHexOperator))]
public class EditorHexOperatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var editorHex = (EditorHexOperator)target;

        // Hex Type Section
        EditorGUILayout.LabelField("Hex Configuration", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        var selectedType = (HexType)EditorGUILayout.EnumPopup("Hex Type", editorHex.SelectedType);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(editorHex, "Select Hex Type");
            editorHex.SelectedType = selectedType;
        }

        if (GUILayout.Button("Apply Type"))
        {
            editorHex.ApplySelectedType();
        }

        EditorGUILayout.Space();

        // Unit Placement Section
        EditorGUILayout.LabelField("Unit Placement", EditorStyles.boldLabel);

        // Show current unit info
        string unitInfo = editorHex.GetUnitInfo();
        EditorGUILayout.LabelField("Current Unit:", unitInfo);

        // Unit selection fields
        EditorGUI.BeginChangeCheck();
        string selectedUnitId = EditorGUILayout.TextField("Unit ID", GetSerializedFieldValue<string>(editorHex, "_selectedUnitId"));
        if (EditorGUI.EndChangeCheck())
        {
            SetSerializedFieldValue(editorHex, "_selectedUnitId", selectedUnitId);
        }

        EditorGUI.BeginChangeCheck();
        int selectedUnitLevel = EditorGUILayout.IntField("Unit Level", GetSerializedFieldValue<int>(editorHex, "_selectedUnitLevel"));
        if (EditorGUI.EndChangeCheck())
        {
            SetSerializedFieldValue(editorHex, "_selectedUnitLevel", Mathf.Max(1, selectedUnitLevel));
        }

        EditorGUI.BeginChangeCheck();
        string selectedPlayerId = EditorGUILayout.TextField("Player ID", GetSerializedFieldValue<string>(editorHex, "_selectedPlayerId"));
        if (EditorGUI.EndChangeCheck())
        {
            SetSerializedFieldValue(editorHex, "_selectedPlayerId", selectedPlayerId);
        }

        EditorGUI.BeginChangeCheck();
        HexDirection selectedDirection = (HexDirection)EditorGUILayout.EnumPopup("Direction", GetSerializedFieldValue<HexDirection>(editorHex, "_selectedUnitDirection"));
        if (EditorGUI.EndChangeCheck())
        {
            SetSerializedFieldValue(editorHex, "_selectedUnitDirection", selectedDirection);
        }

        // Unit action buttons
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = !string.IsNullOrEmpty(selectedUnitId);
        if (GUILayout.Button("Place Unit"))
        {
            editorHex.PlaceUnit();
        }
        GUI.enabled = true;

        GUI.enabled = editorHex.HasUnit();
        if (GUILayout.Button("Remove Unit"))
        {
            editorHex.RemoveUnit();
        }
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }

    private T GetSerializedFieldValue<T>(Object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (T)field?.GetValue(target);
    }

    private void SetSerializedFieldValue<T>(Object target, string fieldName, T value)
    {
        var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(target, value);
    }
}
#endif

