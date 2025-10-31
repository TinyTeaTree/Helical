using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.Editor
{
    public class SOMaker : EditorWindow
    {

        private int _selectedTypeIndex;
        private List<Type> _allSOTypes;
        private string _assetName;
        
        [MenuItem("Custom/SO Maker")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SOMaker));
        }

        private void OnGUI()
        {
            if (_allSOTypes == null)
            {
                _allSOTypes = new();
            }

            if (GUILayout.Button("Refresh"))
            {
                _allSOTypes.Clear();
                var baseType = typeof(BaseSO);
                var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in allAssemblies)
                {
                    var referenced = assembly.GetReferencedAssemblies();
                    if (referenced.Any(a => a.FullName == baseType.Assembly.GetName().FullName))
                    {
                        var allTypes = ReflectionUtils.FindDerivedTypes(assembly, baseType)
                            .Where(t => !t.IsAbstract);
                        _allSOTypes.AddRange(allTypes);

                    }
                }
            }

            if (_allSOTypes.Count > 0)
            {
                var typeNames = _allSOTypes.Select(t => t.Name).ToArray();
                _selectedTypeIndex = EditorGUILayout.Popup(_selectedTypeIndex, typeNames);

                _assetName = EditorGUILayout.TextField("Asset Name : ", _assetName);
                var path = GetSelectedPathOrFallback();
                EditorGUILayout.LabelField("Path : ", path);
                

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Create"))
                    {
                        CreateMyAsset(_allSOTypes[_selectedTypeIndex], _assetName, path);
                    }

                    if (GUILayout.Button("Find"))
                    {
                        FindMyAsset(_allSOTypes[_selectedTypeIndex]);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        public static void FindMyAsset(Type soType)
        {
            var assets = AssetDatabase.FindAssets($"t: {soType.Name}");
            if (assets != null)
            {
                var paths = assets.Select(a => AssetDatabase.GUIDToAssetPath(a));
                var objects = paths.Select(p => AssetDatabase.LoadMainAssetAtPath(p)).ToArray();
                EditorUtility.FocusProjectWindow();
                Selection.objects = objects;
            }
        }
        
        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
		
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if ( !string.IsNullOrEmpty(path) && File.Exists(path) ) 
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
        
        public static void CreateMyAsset(Type soType, string assetName, string path)
        {
            if (assetName.IsNullOrEmpty())
            {
                assetName = soType.Name;
            }
            
            ScriptableObject asset = ScriptableObject.CreateInstance(soType);

            string assetPath = Path.Combine(path, $"{assetName}.asset");
            string name = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AssetDatabase.CreateAsset(asset, name);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}