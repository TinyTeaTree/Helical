using System.Collections;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(WagButton))]
public class WagButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WagButton button = target as WagButton;

        var sound = EditorGUILayout.ObjectField("Click Sound", button._clickSound, typeof(RegularSound), true) as RegularSound;

        if(sound != button._clickSound)
        {
            button._clickSound = sound;
            EditorUtility.SetDirty(target);
        }
    }
}
