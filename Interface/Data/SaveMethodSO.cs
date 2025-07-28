using UnityEngine;

namespace IceCold.SaveService.Interface
{
    public abstract class SaveMethodSO : ScriptableObject, ISaveMethod
    {
        public abstract void SaveProperty(string key, string jsonValue);
        public abstract bool Exists(string key, out string value);
    }
}