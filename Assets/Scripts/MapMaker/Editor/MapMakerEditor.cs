#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapMaker))]
public class MapMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var mapMaker = (MapMaker)target;

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
        {
            if (GUILayout.Button("Populate Level"))
            {
                mapMaker.PopulateLevel();
            }

            if (GUILayout.Button("Clear Level"))
            {
                mapMaker.ClearLevel();
            }
        }
    }
}
#endif

