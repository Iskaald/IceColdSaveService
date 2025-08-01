using System.Threading.Tasks;
using IceCold.SaveService.Interface;
using UnityEngine;

namespace IceCold.SaveService
{
    [CreateAssetMenu(fileName = nameof(PersistentSaveMethod), menuName = "IceCold/Save System/Persistent Method")]
    public class PersistentSaveMethodSO : SaveMethodSO
    {
        public static string Key => nameof(PersistentSaveMethod);
        public string directoryName = "save";
        
        private PersistentSaveMethod persistentSaveMethod;

        public override Task<bool> SaveProperty(string key, string jsonValue)
        {
            persistentSaveMethod ??= new PersistentSaveMethod();
            return persistentSaveMethod?.SaveProperty(key, jsonValue);
        }

        public override bool Exists(string key, out string value)
        {
            persistentSaveMethod ??= new PersistentSaveMethod();
            if (persistentSaveMethod != null) return persistentSaveMethod.Exists(key, out value);
            
            IceColdLogger.LogError("PersistentSaveMethod is not set");
            value = null;
            return false;
        }
    }
}