using UnityEngine;
using UnityEditor;

namespace Game
{
    [CustomPropertyDrawer(typeof(HexAreaPattern))]
    public class HexAreaPatternDrawer : PropertyDrawer
    {
        const int buttonSize = 50;
        const int buttonSpacing = 2;
        const float pixelsPerUnit = 90f; // 1 world unit = 90 pixels in GUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Get properties
            var gridSizeProp = property.FindPropertyRelative("_gridSize");
            var patternDataProp = property.FindPropertyRelative("_patternData");

            // Draw grid size field
            var gridSizeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(gridSizeRect, gridSizeProp);

            // Calculate grid area positioned below the field
            int gridSize = gridSizeProp.intValue;

            // Calculate world bounds for the grid
            int center = gridSize / 2;
            Vector2Int minCoord = new Vector2Int(-center, -center);
            Vector2Int maxCoord = new Vector2Int(center, center);

            // Convert to world XZ coordinates
            Vector2 minWorld = minCoord.ToWorldXZ();
            Vector2 maxWorld = maxCoord.ToWorldXZ();

            // Add some padding
            minWorld -= Vector2.one * 0.5f;
            maxWorld += Vector2.one * 0.5f;

            // Calculate grid dimensions in GUI space
            float worldWidth = maxWorld.x - minWorld.x;
            float worldHeight = maxWorld.y - minWorld.y;
            float gridWidth = worldWidth * pixelsPerUnit;
            float gridHeight = worldHeight * pixelsPerUnit;

            // Position grid below the field, centered horizontally
            float gridStartY = position.y + EditorGUIUtility.singleLineHeight + 10;
            float gridStartX = position.x + (position.width - gridWidth) / 2;

            var gridRect = new Rect(gridStartX, gridStartY, gridWidth, gridHeight);

            // Draw grid background
            EditorGUI.DrawRect(gridRect, new Color(0.2f, 0.2f, 0.2f));

            // Ensure pattern data is the right size
            int expectedSize = gridSize * gridSize;
            if (patternDataProp.arraySize != expectedSize)
            {
                patternDataProp.arraySize = expectedSize;
            }

            // Draw hex grid using world coordinate conversion
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    // Convert grid coordinates to relative world coordinates
                    int relativeX = x - center;
                    int relativeY = y - center;
                    Vector2Int coord = new Vector2Int(relativeX, relativeY);

                    // Convert to world XZ position
                    Vector2 worldPos = coord.ToWorldXZ();

                    // Convert to GUI position relative to grid rect
                    // Center the grid and flip Y so North (increasing y) goes up
                    float guiX = gridRect.x + gridRect.width/2 + (worldPos.x * pixelsPerUnit);
                    float guiY = gridRect.y + gridRect.height/2 + (-worldPos.y * pixelsPerUnit);

                    var buttonRect = new Rect(guiX - buttonSize/2, guiY - buttonSize/2, buttonSize, buttonSize);

                    // Get pattern data
                    int index = y * gridSize + x;
                    var patternItem = patternDataProp.GetArrayElementAtIndex(index);

                    // Highlight center
                    bool isCenter = (x == center && y == center);
                    Color originalColor = GUI.color;
                    if (isCenter)
                    {
                        GUI.color = Color.yellow;
                    }

                    // Draw button with coordinate label
                    bool currentValue = patternItem.boolValue;
                    string labelText = isCenter ? "C" : $"{relativeX},{relativeY}";
                    bool newValue = GUI.Toggle(buttonRect, currentValue, labelText, "Button");

                    // Reset color
                    GUI.color = originalColor;

                    // Update value if changed
                    if (newValue != currentValue)
                    {
                        patternItem.boolValue = newValue;
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var gridSizeProp = property.FindPropertyRelative("_gridSize");
            int gridSize = gridSizeProp.intValue;

            // Calculate height based on world coordinate conversion
            int center = gridSize / 2;
            Vector2Int minCoord = new Vector2Int(-center, -center);
            Vector2Int maxCoord = new Vector2Int(center, center);

            Vector2 minWorld = minCoord.ToWorldXZ();
            Vector2 maxWorld = maxCoord.ToWorldXZ();

            float worldHeight = maxWorld.y - minWorld.y;
            float guiHeight = worldHeight * pixelsPerUnit;

            return EditorGUIUtility.singleLineHeight + guiHeight + 14; // Extra padding
        }
    }
}
