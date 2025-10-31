using System.Collections.Generic;
using System.Linq;
using Game;
using Services;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(GridSO))]
    public class GridSOEditor : UnityEditor.Editor
    {
        private GridSO _target;
        private SerializedProperty _idProperty;
        private SerializedProperty _cellsProperty;
        private SerializedProperty _widthProperty;
        private SerializedProperty _heightProperty;

        private void OnEnable()
        {
            _target = (GridSO)target;
            _idProperty = serializedObject.FindProperty("_id");
            _cellsProperty = serializedObject.FindProperty("_cells");
            _widthProperty = serializedObject.FindProperty("_width");
            _heightProperty = serializedObject.FindProperty("_height");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Seed Options", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Random Seed"))
            {
                SeedRandom();
            }

            if (GUILayout.Button("Perlin Noise Seed"))
            {
                SeedPerlinNoise();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SeedRandom()
        {
            var availableTypes = GetAvailableHexTypes();
            if (availableTypes.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No hex types found in Grid Resource Pack. Please set up the Grid Resource Pack first.", "OK");
                return;
            }

            Undo.RecordObject(_target, "Random Seed Grid");
            
            var cells = new List<HexData>();
            var width = _widthProperty.intValue;
            var height = _heightProperty.intValue;

            if (width <= 0 || height <= 0)
            {
                EditorUtility.DisplayDialog("Error", "Width and Height must be greater than 0 before seeding.", "OK");
                return;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var hexType = availableTypes[Random.Range(0, availableTypes.Count)];
                    cells.Add(new HexData
                    {
                        Coordinate = new Vector2Int(x, y),
                        Type = hexType
                    });
                }
            }

            _target.Cells = cells;
            EditorUtility.SetDirty(_target);
        }

        private void SeedPerlinNoise()
        {
            var availableTypes = GetAvailableHexTypes();
            if (availableTypes.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No hex types found in Grid Resource Pack. Please set up the Grid Resource Pack first.", "OK");
                return;
            }

            // Filter out None type for terrain types
            var terrainTypes = availableTypes.Where(t => t != HexType.None).ToList();
            if (terrainTypes.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No terrain hex types found in Grid Resource Pack. Please add terrain types.", "OK");
                return;
            }

            Undo.RecordObject(_target, "Perlin Noise Seed Grid");
            
            var cells = new List<HexData>();
            var width = _widthProperty.intValue;
            var height = _heightProperty.intValue;

            if (width <= 0 || height <= 0)
            {
                EditorUtility.DisplayDialog("Error", "Width and Height must be greater than 0 before seeding.", "OK");
                return;
            }

            float scale = 0.1f; // Adjust this to change the noise frequency
            float threshold = 0.3f; // Below this threshold, use None, above use terrain

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float noiseValue = Mathf.PerlinNoise(x * scale, y * scale);
                    
                    HexType hexType;
                    if (noiseValue < threshold)
                    {
                        hexType = HexType.None;
                    }
                    else
                    {
                        // Map noise value to terrain types
                        float normalizedValue = (noiseValue - threshold) / (1f - threshold);
                        int typeIndex = Mathf.FloorToInt(normalizedValue * terrainTypes.Count);
                        typeIndex = Mathf.Clamp(typeIndex, 0, terrainTypes.Count - 1);
                        hexType = terrainTypes[typeIndex];
                    }

                    cells.Add(new HexData
                    {
                        Coordinate = new Vector2Int(x, y),
                        Type = hexType
                    });
                }
            }

            _target.Cells = cells;
            EditorUtility.SetDirty(_target);
        }

        private List<HexType> GetAvailableHexTypes()
        {
            // Load the Grid Resource Pack
            var resourcePack = Resources.Load<GridResourcePack>(Addresses.GridResourcePack);
            if (resourcePack == null)
            {
                Debug.LogWarning($"Grid Resource Pack not found at path: {Addresses.GridResourcePack}");
                return new List<HexType> { HexType.None };
            }

            return resourcePack.GetAvailableHexTypes();
        }
    }
}

