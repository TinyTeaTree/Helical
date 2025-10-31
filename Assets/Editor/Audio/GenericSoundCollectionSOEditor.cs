using UnityEditor;
using UnityEngine;

namespace Services.Editor
{
	[CustomEditor(typeof(GenericSoundCollectionSO))]
	public class GenericSoundCollectionSOEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			EditorGUILayout.Space();
			if (GUILayout.Button("Update DJ Generic Sounds"))
			{
				DJGenericAutoUpdater.UpdateFile();
			}
		}
	}
}

