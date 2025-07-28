using System.Linq;
using IceCold.SaveService.Interface;
using UnityEditor;
using UnityEngine;

namespace IceCold.SaveService.Editor
{
    public static class SaveServiceEditorHelper
    {
        private static string saveServiceConfigAssetPath = null;

        public static void ClearData()
        {
            if (string.IsNullOrEmpty(saveServiceConfigAssetPath))
                saveServiceConfigAssetPath = GetAssetPath();
            if (string.IsNullOrEmpty(saveServiceConfigAssetPath))
            {
                IceColdLogger.LogError("There is no config for the SaveSystem. Will clear Player Prefs");
                ClearDataUsingPlayerPrefs();
                return;
            }
            ClearDataUsingMethod();
        }

        private static void ClearDataUsingMethod()
        {
            var config = AssetDatabase.LoadAssetAtPath<SaveServiceConfig>(saveServiceConfigAssetPath);
            if (config == null)
            {
                IceColdLogger.LogError("There is no config for the SaveSystem. Will clear Player Prefs");
                ClearDataUsingPlayerPrefs();
                return;
            }
            
            var saveMethod = config.saveMethod;
            if (saveMethod == null)
            {
                IceColdLogger.LogError("Save method not available. Defaulting to PlayerPrefs for save.");
                ClearDataUsingPlayerPrefs();
                return;
            }

            switch (saveMethod)
            {
                case LocalSaveMethodSO:
                    LocalSaveMethod.Clear();
                    break;
                case PersistentSaveMethodSO persistentSaveMethodSO:
                    PersistentSaveMethod.Clear(persistentSaveMethodSO.directoryName);
                    break;
                default:
                    IceColdLogger.LogError("Save method not available. Defaulting to PlayerPrefs for save.");
                    ClearDataUsingPlayerPrefs();
                    break;
            }
        }

        private static void ClearDataUsingPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            IceColdLogger.Log("Cleared all save data (PlayerPrefs).");
        }

        private static string GetAssetPath()
        {
            var guids = AssetDatabase.FindAssets("t:SaveServiceConfig");
            return guids.Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault();
        }
    }
}