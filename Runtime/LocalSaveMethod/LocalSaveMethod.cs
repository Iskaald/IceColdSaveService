using System.Threading.Tasks;
using IceCold.SaveService.Interface;
using UnityEngine;

namespace IceCold.SaveService
{
    public class LocalSaveMethod : ISaveMethod
    {
        public bool Exists(string key, out string value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                try
                {
                    value = PlayerPrefs.GetString(key);
                    return true;
                }
                catch
                {
                    value = null;
                    return false;
                }
            }

            value = null;
            return false;
        }

        public Task<bool> SaveProperty(string key, string jsonValue)
        {
            PlayerPrefs.SetString(key, jsonValue);
            PlayerPrefs.Save();
            return Task.FromResult(true);
        }
        
        public static void Clear()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            IceColdLogger.Log("Cleared all local save data (PlayerPrefs).");
        }
    }
}