using System;
using System.Linq;
using System.Reflection;
using IceCold.SaveService.Interface;
using UnityEditor;
using UnityEngine;

namespace IceCold.SaveService.Editor
{
    public class SaveMethodCreatorWindow : EditorWindow
    {
        private SaveServiceConfig _config;
        private Type[] _saveMethodTypes;
        private Type[] _typesToCreate;

        public static void ShowWindow(SaveServiceConfig config)
        {
            var window = GetWindow<SaveMethodCreatorWindow>(true, "Create Save Method");
            window._config = config;
            window.FindSaveMethodTypes();
            window.Show();
        }

        private void FindSaveMethodTypes()
        {
            var guids = AssetDatabase.FindAssets("t:SaveMethodSO");
            var existingTypes = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<SaveMethodSO>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(sm => sm != null)
                .Select(sm => sm.GetType())
                .ToHashSet();

            _saveMethodTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .Where(t =>
                        typeof(SaveMethodSO).IsAssignableFrom(t) &&
                        !t.IsAbstract &&
                        t.GetCustomAttribute<CreateAssetMenuAttribute>() != null &&
                        !existingTypes.Contains(t)
                )
                .ToArray();
        }

        private void OnGUI()
        {
            if (_saveMethodTypes == null || _saveMethodTypes.Length == 0)
            {
                EditorGUILayout.HelpBox(
                    "All SaveMethodSO types already have assets in the project, or no SaveMethodSO implementations with [CreateAssetMenu] are present.",
                    MessageType.Info
                );
                if (GUILayout.Button("Refresh")) FindSaveMethodTypes();
                return;
            }

            EditorGUILayout.LabelField("Select Save Method type to create:", EditorStyles.boldLabel);

            foreach (var type in _saveMethodTypes)
            {
                var attr = type.GetCustomAttribute<CreateAssetMenuAttribute>();
                string label;
                if (attr != null && !string.IsNullOrEmpty(attr.menuName))
                {
                    var parts = attr.menuName.Split('/');
                    label = parts[^1].Trim();
                }
                else
                {
                    label = type.Name;
                }
                if (GUILayout.Button(label))
                {
                    CreateAndAssignSaveMethod(type, label);
                    Close();
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Refresh List")) FindSaveMethodTypes();
        }

        private void CreateAndAssignSaveMethod(Type type, string label)
        {
            const string folderPath = "Assets/IceCold/Settings/Resources/SaveMethods";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                if (!AssetDatabase.IsValidFolder("Assets/IceCold"))
                    AssetDatabase.CreateFolder("Assets", "IceCold");
                if (!AssetDatabase.IsValidFolder("Assets/IceCold/Settings"))
                    AssetDatabase.CreateFolder("Assets/IceCold", "Settings");
                if (!AssetDatabase.IsValidFolder("Assets/IceCold/Settings/Resources"))
                    AssetDatabase.CreateFolder("Assets/IceCold/Settings", "Resources");
                if (!AssetDatabase.IsValidFolder(folderPath))
                    AssetDatabase.CreateFolder("Assets/IceCold/Settings/Resources", "SaveMethods");
            }

            var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{type.Name}.asset");
            var asset = CreateInstance(type);
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Undo.RecordObject(_config, "Create Save Method");
            var saveMethodSO = asset as SaveMethodSO;
            _config.AddSaveMethod(saveMethodSO);
            EditorUtility.SetDirty(_config);
        }
    }
}