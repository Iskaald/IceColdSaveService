using IceCold.Interface;
using UnityEngine;

namespace IceCold.SaveService.Interface
{
    [CreateAssetMenu(fileName = "SaveServiceConfig", menuName = "IceCold/Save System/Create Config", order = 0)]
    public class SaveServiceConfig : IceColdConfig
    {
        public static string ConfigKey => nameof(SaveServiceConfig);
        public override string Key => ConfigKey;

        [Header("Save Strategy")]
        public SaveStrategy saveStrategy = SaveStrategy.Auto;
        public float saveIntervalInSeconds = 180f;
        
        [Header("Save Method")]
        public SaveMethodSO saveMethod;
        
        public void AddSaveMethod (SaveMethodSO method)
        {
            if (!saveMethod)
                saveMethod = method;
        }
    }
}