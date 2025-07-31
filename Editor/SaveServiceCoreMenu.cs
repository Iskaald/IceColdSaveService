using IceCold.Editor;
using IceCold.SaveService.Interface;
using UnityEditor;
using UnityEngine;

namespace IceCold.SaveService.Editor
{
    public class SaveServiceCoreMenu : IceColdMenu
    {
        [MenuItem("IceCold/Save Service/Config", priority = 10)]
        private static void SelectSaveServiceConfig()
        {
            var config = FindConfigAsset<SaveServiceConfig>();
            
            if (config != null)
            {
                Selection.activeObject = config;
                EditorGUIUtility.PingObject(config);
            }
        }
        
        [MenuItem("IceCold/Save Service/", priority = 11)]
        private static void Separator() { }
        
        [MenuItem("IceCold/Save Service/Clear Saved Data", priority = 12)]
        private static void ClearSavedData()
        {
            if (!Application.isPlaying)
            {
                SaveServiceEditorHelper.ClearData();
            }
            else
            {
                IceColdLogger.LogWarning("Cannot clear save data while playing.");
            }
        }

        [MenuItem("IceCold/Save Service/Clear Saved Data", true)]
        private static bool ValidateClearSavedData()
        {
            return !Application.isPlaying;
        }
    }
}