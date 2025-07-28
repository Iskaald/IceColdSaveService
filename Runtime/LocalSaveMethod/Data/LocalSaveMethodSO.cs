using IceCold.SaveService.Interface;
using UnityEngine;

namespace IceCold.SaveService
{
    [CreateAssetMenu(fileName = "LocalSaveMethod", menuName = "IceCold/Save System/Local Method")]

    public class LocalSaveMethodSO : SaveMethodSO
    {
        private LocalSaveMethod localSaveMethod;

        public override void SaveProperty(string key, string jsonValue)
        {
            localSaveMethod ??= new LocalSaveMethod();
            localSaveMethod.SaveProperty(key, jsonValue);
        }

        public override bool Exists(string key, out string value)
        {
            localSaveMethod ??= new LocalSaveMethod();
            return localSaveMethod.Exists(key, out value);
        }
    }
}