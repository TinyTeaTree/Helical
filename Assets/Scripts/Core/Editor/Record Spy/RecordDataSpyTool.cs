using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class RecordDataSpyTool : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    private BaseRecord _record = null;

    private static GUIStyle textStyle;


    [MenuItem("Custom/Record Spy")]
    public static void OpenWindow()
    {
        RecordDataSpyTool window = (RecordDataSpyTool)EditorWindow.GetWindow(typeof(RecordDataSpyTool));
        window.Show();
        
        AssureTextStyle();
    }

    private static void AssureTextStyle()
    {
        if (textStyle != null)
            return;

        textStyle = new GUIStyle(EditorStyles.label);
        textStyle.wordWrap = true;
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            GUILayout.Label("Application is not playing now");
            return;
        }

        AssureTextStyle();

        var gameClient = GameInfra.Single;
        if (gameClient == null)
        {
            GUILayout.Label("GameInfra is not instantiated now");
            return;
        }

        var dataRecords = gameClient.Records;
        if (dataRecords == null)
        {
            GUILayout.Label("No data records to explore");
            return;
        }
        
        DrawDataRecords(dataRecords);
    }

    private void DrawDataRecords(Dictionary<Type, BaseRecord> records)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));

        foreach (var t in records.Keys)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            {
                if (GUILayout.Button(t.Name))
                {
                    if (_record == records[t])
                    {
                        _record = null;
                    }
                    else
                    {
                        _record = records[t];
                    }
                }

                if (_record != null && _record == records[t])
                {
                    DrawRecord(_record, 0);
                }
            }
            GUILayout.EndVertical();


            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void DrawRecord(object record, int level)
    {
        if (level >= 10)
        {
            return; //Too deep
        }
        
        var recordType = record.GetType();
        var publicProps = recordType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop => prop.CanWrite && prop.CanRead);
        var publicFields = recordType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        
        foreach (var prop in publicProps)
        {
            var value = prop.GetValue(record);
            DrawPropVisual(prop.Name, value, level);
        }

        foreach (var field in publicFields)
        {
            var value = field.GetValue(record);
            DrawPropVisual(field.Name, value, level);
        }
    }

    private void DrawPropVisual(string propName, object prop, int level)
    {
        if (level >= 10)
        {
            return; //Too deep
        }

        if (prop == null)
        {
            EditorGUILayout.TextField($"{propName} - null", textStyle);
            return;
        }
        
        var propType = prop.GetType();
        if (!propType.IsClass || propType == typeof(string) || propType == typeof(JObject) || propType == typeof(JToken))
        {
            EditorGUILayout.TextField($"{propName} - {prop}", textStyle);
        }
        else if (prop is IEnumerable enumerable && prop is ICollection)
        {
            EditorGUI.indentLevel++;
            {
                var index = 0;
                var hasElements = false;
                foreach (var e in enumerable)
                {
                    hasElements = true;
                    DrawPropVisual($"{propName}[{index}]", e, level + 1);
                    index++;
                }

                if (!hasElements)
                {
                    EditorGUILayout.TextField($"{propName} - empty", textStyle);
                }
            }
            EditorGUI.indentLevel--;
        }
        else
        {
            if (ShouldDrillInto(propType))
            {
                EditorGUILayout.TextField($"{propName} - {prop}", textStyle);
                EditorGUI.indentLevel++;
                {
                    DrawRecord(prop, level + 1);
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.TextField($"{propName} - {prop}", textStyle);
            }
        }
    }

    private static bool ShouldDrillInto(Type type)
    {
        if (type == null)
        {
            return false;
        }

        if (!type.IsClass)
        {
            return false;
        }

        if (type == typeof(string) || typeof(JToken).IsAssignableFrom(type))
        {
            return false;
        }

        var assemblyName = type.Assembly.GetName().Name;
        var baseRecordAssemblyName = typeof(BaseRecord).Assembly.GetName().Name;

        if (assemblyName == baseRecordAssemblyName)
        {
            return true;
        }

        if (assemblyName.StartsWith("System", StringComparison.OrdinalIgnoreCase) ||
            assemblyName.StartsWith("Unity", StringComparison.OrdinalIgnoreCase) ||
            assemblyName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) ||
            assemblyName.Equals("netstandard", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var ns = type.Namespace ?? string.Empty;
        if (ns.StartsWith("System", StringComparison.Ordinal) ||
            ns.StartsWith("UnityEngine", StringComparison.Ordinal) ||
            ns.StartsWith("UnityEditor", StringComparison.Ordinal) ||
            ns.StartsWith("TMPro", StringComparison.Ordinal) ||
            ns.StartsWith("Newtonsoft", StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }
}