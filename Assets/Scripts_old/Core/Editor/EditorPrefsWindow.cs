using UnityEditor;
using UnityEngine;

public class EditorPrefsWindow : EditorWindow
{
    string key;
    string value;

    [MenuItem("Window/Editor Prefs")]
    static void Init()
    {
        var window = GetWindow<EditorPrefsWindow>();
        window.Show();
    }

    void OnGUI()
    {
        key = EditorGUILayout.TextField("Key", key);
        value = EditorGUILayout.TextField("Value", value);

        if (GUILayout.Button("Set EditorPref"))
        {
            EditorPrefs.SetString(key, value);
        }

        if (GUILayout.Button("Delete EditorPref"))
        {
            EditorPrefs.DeleteKey(key);
        }
    }
}