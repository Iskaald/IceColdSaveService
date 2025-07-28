using System;
using System.IO;
using IceCold.SaveService.Interface;
using UnityEngine;

namespace IceCold.SaveService
{
    public class PersistentSaveMethod : ISaveMethod
    {
        private PersistentSaveMethodSO config;
        private readonly string path;
        
        public PersistentSaveMethod()
        {
            config = Resources.Load<PersistentSaveMethodSO>(PersistentSaveMethodSO.Key);
            path = Path.Combine(Application.persistentDataPath, (config != null ? config.directoryName : "save"));
            
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        
        public void SaveProperty(string key, string jsonValue)
        {
            try
            {
                var filePath = GetFilePath(key);
                File.WriteAllText(filePath, jsonValue);
            }
            catch (IOException e)
            {
                IceColdLogger.LogWarning($"Failed to save {key} to persistent storage: {e.Message}");
            }
            catch (Exception e)
            {
                IceColdLogger.LogWarning($"Failed to save {key} to persistent storage: {e.Message}");
            }
        }
        
        public bool Exists(string key, out string value)
        {
            value = LoadProperty(key);
            return value != null;
        }
        
        public static void Clear(string directoryName)
        {
            var saveLoc = Path.Combine(Application.persistentDataPath, directoryName);
            if (File.Exists(saveLoc))
            {
                File.Delete(saveLoc);
            }
            IceColdLogger.Log("Cleared persistent save data.");
        }
        
        private string LoadProperty(string key)
        {
            try
            {
                var filePath = GetFilePath(key);
                if (File.Exists(filePath))
                {
                    var content = File.ReadAllText(filePath);
                    IceColdLogger.Log($"Loaded property '{key}' from file: {filePath}");
                    return content;
                }
            }
            catch (Exception e)
            {
                IceColdLogger.LogError($"Failed to load '{key}': {e}");
            }

            return null;
        }
        
        private string GetFilePath(string key)
        {
            var safeKey = string.Join("_", key.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(path, safeKey + ".json");
        }
    }
}