using System.Reflection;
using IceCold.SaveService.Interface;
using UnityEngine;

namespace IceCold.SaveService.Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(SaveServiceConfig))]
    public class SaveServiceConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var config = (SaveServiceConfig)target;

            var type = config.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null)
                    continue;

                if (field.Name == nameof(SaveServiceConfig.saveIntervalInSeconds))
                {
                    if (config.saveStrategy == SaveStrategy.Interval)
                    {
                        var intervalProp = serializedObject.FindProperty(field.Name);
                        EditorGUILayout.PropertyField(intervalProp);
                    }
                }
                else
                {
                    var prop = serializedObject.FindProperty(field.Name);
                    if (prop != null)
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }
                }
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Save Method"))
            {
                SaveMethodCreatorWindow.ShowWindow(config);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}