using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using UnityEngine;
using ChessRaid.LevelBuilder;

[CustomEditor(typeof(HexPlacer))]
public class MyComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector interface

        HexPlacer placer = (HexPlacer)target;

        // Add a button to the custom inspector
        if (GUILayout.Button("Save"))
        {
            placer.Save(); // Call the method on button press
        }
    }
}