using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(RangeFloat))]
public class RangeFloatPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        var pos = position;

        EditorGUI.LabelField(new Rect(pos.x, pos.y, 100, 20), label);
        pos.x += 100; //width
        pos.x += 20; //buffer
        EditorGUI.LabelField(new Rect(pos.x, pos.y, 40, 20), "From : ");
        pos.x += 40; //width;

        var _fromRect = new Rect(pos.x, pos.y, 50, pos.height);
        EditorGUI.PropertyField(_fromRect, property.FindPropertyRelative("_from"), GUIContent.none);
        pos.x += 50; //width;
        pos.x += 10; //buffer;

        EditorGUI.LabelField(new Rect(pos.x, pos.y, 40, 20), "To : ");
        pos.x += 40; //width;
        var _toRect = new Rect(pos.x, pos.y, 50, pos.height);
        EditorGUI.PropertyField(_toRect, property.FindPropertyRelative("_to"), GUIContent.none);

        EditorGUI.EndProperty();
    }
}
