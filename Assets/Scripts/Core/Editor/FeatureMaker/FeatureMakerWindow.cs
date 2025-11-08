using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.FeatureMaker
{
    public class FeatureMakerWindow : EditorWindow
    {
        private string _featureName;
        private bool _isVisual;
        private bool _withRecord;
        private bool _withConfig;
        private bool _withSO;

        private string _serviceName;

        private string _agentName;

        [MenuItem("Custom/Feature Maker")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(FeatureMakerWindow));
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    _featureName = EditorGUILayout.TextField("Feature Name : ", _featureName);
                    _isVisual = EditorGUILayout.Toggle("Visual Feature", _isVisual);
                    _withRecord = EditorGUILayout.Toggle("With Data Record", _withRecord);
                    
                    _withConfig = EditorGUILayout.Toggle("With Config", _withConfig);
                    if (_withConfig)
                    {
                        _withSO = EditorGUILayout.Toggle("SO For Config", _withSO);
                    }

                    if (GUILayout.Button("Create Feature"))
                    {
                        CreateFeature();
                    }
                }
                GUILayout.EndVertical();
                
                GUILayout.Space(20f);
                GUILayout.VerticalSlider(0f, 0f, 1f);
                GUILayout.Space(20f);
                
                GUILayout.BeginVertical();
                {
                    _serviceName = EditorGUILayout.TextField("Service Name : ", _serviceName);

                    if (GUILayout.Button("Create Service"))
                    {
                        CreateService();
                    }
                }
                GUILayout.EndVertical();
                
                GUILayout.BeginVertical();
                {
                    _agentName = EditorGUILayout.TextField("Agent Name : ", _agentName);

                    if (GUILayout.Button("Create Agent"))
                    {
                        CreateAgent();
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            // if (GUILayout.Button("Collect Addresses"))
            // {
            //     UpdateAddresses();
            // }
        }

        private void CreateAgent()
        {
            if (_agentName.IsNullOrEmpty())
            {
                Debug.LogError("Can't create null name");
                return;
            }

            if (_agentName.Contains("Agent"))
            {
                _serviceName = _serviceName.Replace("Agent", "");
            }
            
            if (_agentName.IsNullOrEmpty())
            {
                Debug.LogError("Can't create null name");
                return;
            }

            var chars = _agentName.ToCharArray();
            foreach (var c in chars)
            {
                if (!Char.IsLetter(c))
                {
                    Debug.LogError("Must all be letters");
                    return;
                }
            }

            if (!Char.IsUpper(chars[0]))
            {
                Debug.LogError("First Char must be Upper");
                return;
            }

            var dataPath = Application.dataPath;
            var agentPath = Path.Combine(dataPath, "Scripts", "Agents");

            var directories = Directory.GetDirectories(agentPath);

            foreach (var dir in directories)
            {
                var directoryName = Path.GetFileName(dir);

                if (directoryName == _agentName)
                {
                    Debug.LogError("This service already exists");
                    return;
                }
            }

            var agentDirectoryPath = Path.Combine(agentPath, _agentName);
            var agentDirectory = Directory.CreateDirectory(agentDirectoryPath);

            var iAgentTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "IAgent.txt"));
            var iAgentText = iAgentTemplate.Replace("<AgentName>", $"{_agentName}");
            File.WriteAllText(Path.Combine(agentDirectoryPath, $"I{_agentName}Agent.cs"), iAgentText);


            var agentTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "Agent.txt"));
            var agentText = agentTemplate.Replace("<AgentName>", $"{_agentName}");
            File.WriteAllText(Path.Combine(agentDirectoryPath, $"{_agentName}Agent.cs"), agentText);
            

            var infraGuid = AssetDatabase.FindAssets("t:Script GameBootstrap")[0];
            var infraPath = AssetDatabase.GUIDToAssetPath(infraGuid);
            var lines = File.ReadAllLines(infraPath).ToList();

            for (int i = 0; i < lines.Count; ++i)
            {
                var line = lines[i];

                if (line.Contains("//<New Agent>"))
                {
                    lines.Insert(i, line.Replace("//<New Agent>", "") + $"_agents.Add<I{_agentName}Agent>(new {_agentName}Agent());");
                    break;
                }
            }
            
            File.WriteAllLines(infraPath, lines.ToArray());
            
            AssetDatabase.Refresh();
        }

        [MenuItem("Custom/Update Addresses")]
        public static void UpdateAddresses()
        {
            var path = Application.dataPath;
            var addressesGuid = AssetDatabase.FindAssets("t:Script Addresses")[0];
            var addressesPath = AssetDatabase.GUIDToAssetPath(addressesGuid);
            var lines = File.ReadAllLines(addressesPath).ToList();

            List<string> resourceFolders = new();
            GetAllResourceFolders(resourceFolders, path);

            bool startRemoving = false;
            for (int i = lines.Count - 1; i >= 0; --i)
            {
                if (!startRemoving)
                {
                    if (lines[i].Contains("//<Resource Path End>"))
                    {
                        startRemoving = true;
                    }
                }
                else
                {
                    if (lines[i].Contains("//<Resource Path Start>"))
                    {
                        break;
                    }
                    
                    lines.RemoveAt(i);
                }
            }

            foreach (var resourceFolder in resourceFolders)
            {
                List<string> filePaths = new();
                GetAllFiles(filePaths, resourceFolder);

                foreach (var file in filePaths)
                {
                    var extension = Path.GetExtension(file);
                    var filePath = file.Replace(extension, "");
                    var fileName = Path.GetFileName(filePath).Replace(" ", "");
                    var split = filePath.Split("Resources")[^1];
                    var loadPath = split.TrimStart('/', '\\');
                    loadPath = loadPath.Replace('\\', '/');


                    for (int i = 0; i < lines.Count; ++i)
                    {
                        var line = lines[i];

                        if (line.Contains("//<Resource Path End>"))
                        {
                            lines.Insert(i, line.Replace("//<Resource Path End>", "") + $"public const string {fileName} = \"{loadPath}\";");
                            break;
                        }
                    }
            
                }
            }
            
            File.WriteAllLines(addressesPath, lines.ToArray());
            AssetDatabase.Refresh();
        }

        private static void GetAllResourceFolders(List<string> resourceFolders, string path)
        {
            var directories = Directory.GetDirectories(path);

            foreach (var dirPath in directories)
            {
                if (Path.GetFileName(dirPath) == "Resources")
                {
                    if (dirPath.Contains("TextMesh"))
                    {
                        continue; //Ignore
                    }
                    if (dirPath.ToLower().Contains("dotween"))
                    {
                        continue; //Ignore
                    }
                    
                    resourceFolders.Add(dirPath);
                }
                else
                {
                    GetAllResourceFolders(resourceFolders, dirPath);
                }
            }
        }

        private static void GetAllFiles(List<string> filePaths, string path)
        {
            var files = Directory.GetFiles(path);
            foreach (var filePath in files)
            {
                if (Path.GetExtension(filePath) == ".meta")
                {
                    //skip
                }
                else if (filePath.ToLower().Contains("dotween"))
                {
                    //skip
                }
                else
                {
                    filePaths.Add(filePath);
                }
            }

            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                GetAllFiles(filePaths, dir);
            }
        }

        private void CreateService()
        {
            if (_serviceName.IsNullOrEmpty())
            {
                Debug.LogError("Can't create null name");
                return;
            }

            if (_serviceName.Contains("Service"))
            {
                _serviceName = _serviceName.Replace("Service", "");
            }
            
            if (_serviceName.IsNullOrEmpty())
            {
                Debug.LogError("Can't create null name");
                return;
            }

            var chars = _serviceName.ToCharArray();
            foreach (var c in chars)
            {
                if (!Char.IsLetter(c))
                {
                    Debug.LogError("Must all be letters");
                    return;
                }
            }

            if (!Char.IsUpper(chars[0]))
            {
                Debug.LogError("First Char must be Upper");
                return;
            }

            var dataPath = Application.dataPath;
            var servicePath = Path.Combine(dataPath, "Scripts", "Services");

            var directories = Directory.GetDirectories(servicePath);

            foreach (var dir in directories)
            {
                var directoryName = Path.GetFileName(dir);

                if (directoryName == _serviceName)
                {
                    Debug.LogError("This service already exists");
                    return;
                }
            }

            var serviceDirectoryPath = Path.Combine(servicePath, _serviceName);
            var serviceDirectory = Directory.CreateDirectory(serviceDirectoryPath);

            var iServiceTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "IService.txt"));
            var iServiceText = iServiceTemplate.Replace("<ServiceName>", $"{_serviceName}");
            File.WriteAllText(Path.Combine(serviceDirectoryPath, $"I{_serviceName}Service.cs"), iServiceText);


            var serviceTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "Service.txt"));
            var serviceText = serviceTemplate.Replace("<ServiceName>", $"{_serviceName}");
            File.WriteAllText(Path.Combine(serviceDirectoryPath, $"{_serviceName}Service.cs"), serviceText);
            

            var infraGuid = AssetDatabase.FindAssets("t:Script GameBootstrap")[0];
            var infraPath = AssetDatabase.GUIDToAssetPath(infraGuid);
            var lines = File.ReadAllLines(infraPath).ToList();

            for (int i = 0; i < lines.Count; ++i)
            {
                var line = lines[i];

                if (line.Contains("//<New Service>"))
                {
                    lines.Insert(i, line.Replace("//<New Service>", "") + $"_services.Add<I{_serviceName}Service>(new {_serviceName}Service());");
                    break;
                }
            }
            
            File.WriteAllLines(infraPath, lines.ToArray());
            
            AssetDatabase.Refresh();
        }
        
        private void CreateFeature()
        {
            if (_featureName.IsNullOrEmpty())
            {
                Debug.LogError("Can't create null name");
                return;
            }

            if (_featureName.Contains("Feature"))
            {
                _featureName = _featureName.Replace("Feature", "");
            }
            
            if (_featureName.IsNullOrEmpty())
            {
                Debug.LogError("Can't create null name");
                return;
            }

            var chars = _featureName.ToCharArray();
            foreach (var c in chars)
            {
                if (!Char.IsLetter(c))
                {
                    Debug.LogError("Must all be letters");
                    return;
                }
            }

            if (!Char.IsUpper(chars[0]))
            {
                Debug.LogError("First Char must be Upper");
                return;
            }

            var featureClassName = $"{_featureName}Feature";
            var dataPath = Application.dataPath;
            var featurePath = Path.Combine(dataPath, "Scripts", "Features");

            if (!Directory.Exists(featurePath))
            {
                Directory.CreateDirectory(featurePath);
            }

            var directories = Directory.GetDirectories(featurePath);

            bool featureExists = false;
            bool recordExists = false;
            foreach (var dir in directories)
            {
                var directoryName = Path.GetFileName(dir);

                if (directoryName == _featureName)
                {
                    featureExists = true;
                    Debug.LogWarning("This feature already exists");
                }
            }

            var featureDirectoryPath = Path.Combine(featurePath, _featureName);

            if (!featureExists)
            {
                var featureDirectory = Directory.CreateDirectory(featureDirectoryPath);
                var iFeatureTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "IFeature.txt"));
                var iFeatureText = iFeatureTemplate.Replace("<FeatureName>", $"{_featureName}");
                File.WriteAllText(Path.Combine(featureDirectoryPath, $"I{_featureName}.cs"), iFeatureText);
            }
            
            if (_withRecord)
            {
                var recordPath = Path.Combine(featureDirectoryPath, $"{_featureName}Record.cs");
                if (!File.Exists(recordPath))
                {
                    var recordTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "Record.txt"));
                    var recordText = recordTemplate.Replace("<FeatureName>", $"{_featureName}");
                    File.WriteAllText(recordPath, recordText);
                }
                else
                {
                    recordExists = true;
                    Debug.LogWarning("This Record already exists");
                }
            }

            if (_withConfig)
            {
                var configPath = Path.Combine(featureDirectoryPath, $"{_featureName}Config.cs");
                if (!File.Exists(configPath))
                {
                    var configTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "Config.txt"));
                    var configText = configTemplate.Replace("<FeatureName>", $"{_featureName}");
                    File.WriteAllText(configPath, configText);
                }
                else
                {
                    Debug.LogWarning("This Config already exists");
                }

                if (_withSO)
                {
                    var soPath = Path.Combine(featureDirectoryPath, $"{_featureName}SO.cs");
                    if (!File.Exists(soPath))
                    {
                        var configSOTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "ConfigSO.txt"));
                        var configSOText = configSOTemplate.Replace("<FeatureName>", $"{_featureName}");
                        File.WriteAllText(soPath, configSOText);
                    }
                    else
                    {
                        Debug.LogWarning("This Config SO already exists");
                    }
                }
            }

            if (_isVisual)
            {
                var visualPath = Path.Combine(featureDirectoryPath, $"{_featureName}Visual.cs");
                if (!File.Exists(visualPath))
                {
                    var visualTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "Visual.txt"));
                    var visualText = visualTemplate.Replace("<FeatureName>", $"{_featureName}");
                    File.WriteAllText(visualPath, visualText);

                    if (!featureExists)
                    {
                        var visualFeatureTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "VisualFeature.txt"));
                        var visualFeatureText = visualFeatureTemplate.Replace("<FeatureClassName>", $"{featureClassName}")
                                                                       .Replace("<FeatureName>", $"{_featureName}");

                        if (_withRecord)
                        {
                            visualFeatureText = visualFeatureText.Replace("//<Extra>", $"[Inject] public {_featureName}Record Record {{ get; set; }}");
                        }
                        else
                        {
                            visualFeatureText = visualFeatureText.Replace("//<Extra>", "");
                        }

                        var featureScriptPath = Path.Combine(featureDirectoryPath, $"{featureClassName}.cs");
                        File.WriteAllText(featureScriptPath, visualFeatureText);
                    }
                }
            }
            else
            {
                if (!featureExists)
                {
                    var featureTemplate = File.ReadAllText(Path.Combine(dataPath, "Editor", "FeatureTemplate", "Feature.txt"));
                    var featureText = featureTemplate.Replace("<FeatureClassName>", $"{featureClassName}")
                                                     .Replace("<FeatureName>", $"{_featureName}");

                    if (_withRecord)
                    {
                        featureText = featureText.Replace("//<Extra>", $"[Inject] public {_featureName}Record Record {{ get; set; }}");
                    }
                    else
                    {
                        featureText = featureText.Replace("//<Extra>", "");
                    }

                    var featureScriptPath = Path.Combine(featureDirectoryPath, $"{featureClassName}.cs");
                    File.WriteAllText(featureScriptPath, featureText);
                }
            }


            var infraGuid = AssetDatabase.FindAssets("t:Script GameBootstrap")[0];
            var infraPath = AssetDatabase.GUIDToAssetPath(infraGuid);
            var lines = File.ReadAllLines(infraPath).ToList();

            if (!featureExists)
            {
                for (int i = 0; i < lines.Count; ++i)
                {
                    var line = lines[i];

                    if (line.Contains("//<New Feature>"))
                    {
                        lines.Insert(i, line.Replace("//<New Feature>", "") + $"_features.Add<I{_featureName}>(new {featureClassName}());");
                        break;
                    }
                }
            }
            
            if (_withRecord && !recordExists)
            {
                for (int i = 0; i < lines.Count; ++i)
                {
                    var line = lines[i];

                    if (line.Contains("//<New Record>"))
                    {
                        lines.Insert(i, line.Replace("//<New Record>", "") + $"_records.Add(typeof({_featureName}Record), new {_featureName}Record());");
                        break;
                    }
                }
            }
            
            File.WriteAllLines(infraPath, lines.ToArray());
            
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            var obj = Selection.activeObject = AssetDatabase.LoadAssetAtPath<DefaultAsset>(Path.Combine("Assets", "Scripts", "Features", _featureName));
            EditorGUIUtility.PingObject(obj);
        }
    }
}