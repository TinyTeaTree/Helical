using UnityEngine;
using UnityEditor;
using Game;

[CustomEditor(typeof(BattleUnitConfigSO))]
public class BattleUnitConfigSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BattleUnitConfigSO battleUnitConfigSO = (BattleUnitConfigSO)target;

        // Add some spacing
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

        // Add button to add this SO to the main BattleUnitsSO
        if (GUILayout.Button("Add to Main BattleUnits Config"))
        {
            AddToMainBattleUnitsConfig(battleUnitConfigSO);
        }

        EditorGUILayout.HelpBox("Click to automatically add this battle unit to the main BattleUnitsSO configuration.", MessageType.Info);
    }

    private void AddToMainBattleUnitsConfig(BattleUnitConfigSO battleUnitConfigSO)
    {
        // Find the main BattleUnitsSO asset
        string[] guids = AssetDatabase.FindAssets("t:BattleUnitsSO");
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No BattleUnitsSO found in the project!", "OK");
            return;
        }

        if (guids.Length > 1)
        {
            EditorUtility.DisplayDialog("Error", "Multiple BattleUnitsSO assets found! There should only be one.", "OK");
            return;
        }

        // Load the BattleUnitsSO
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        BattleUnitsSO battleUnitsSO = AssetDatabase.LoadAssetAtPath<BattleUnitsSO>(path);

        if (battleUnitsSO == null)
        {
            EditorUtility.DisplayDialog("Error", "Failed to load BattleUnitsSO!", "OK");
            return;
        }

        // Get the config
        var config = battleUnitsSO.Config as BattleUnitsConfig;
        if (config == null)
        {
            EditorUtility.DisplayDialog("Error", "BattleUnitsSO config is not a BattleUnitsConfig!", "OK");
            return;
        }

        // Check if this SO is already in the list
        if (config.BattleUnitConfigs.Contains(battleUnitConfigSO))
        {
            EditorUtility.DisplayDialog("Already Added", $"This battle unit ({battleUnitConfigSO.Config.Id}) is already in the main config!", "OK");
            return;
        }

        // Add to the list
        config.BattleUnitConfigs.Add(battleUnitConfigSO);

        // Mark as dirty and save
        EditorUtility.SetDirty(battleUnitsSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Added battle unit '{battleUnitConfigSO.Config.Id}' to main BattleUnitsSO");
        EditorUtility.DisplayDialog("Success", $"Added '{battleUnitConfigSO.Config.Id}' to the main BattleUnitsSO configuration!", "OK");
    }
}
