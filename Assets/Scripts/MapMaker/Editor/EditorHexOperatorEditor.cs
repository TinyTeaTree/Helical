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
    }
}
#endif

