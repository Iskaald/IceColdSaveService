using System;
using System.Threading.Tasks;
using IceCold.SaveService.Interface;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;

namespace IceCold.SaveService
{
    public class Property<T> : IProperty<T>
    {
        private readonly string key;
        private T value;
        
        private ISaveMethod saveMethod;
        
        public T Value
        {
            get => value;
            set => this.value = value;
        }

        public string Key => key;

        object IProperty.Value
        {
            get => Value;
            set => Value = (T) value;
        }

        public Task<bool> Save()
        {
            var json = JsonConvert.SerializeObject(new SaveWrapper<T> { v = value });
            
            if (saveMethod == null)
            {
                IceColdLogger.LogWarning("Save method not available. Defaulting to PlayerPrefs for save.");
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
                return Task.FromResult(false);
            }
            
            return saveMethod?.SaveProperty(key, json);
        }

        public Property(string key, T defaultValue, ISaveMethod method)
        {
            this.key = key;
            saveMethod = method;
            LoadOrSetDefault(defaultValue);
        }

        private void LoadOrSetDefault(T defaultValue)
        {
            if (saveMethod.Exists(key, out var jsonValue))
            {
                try
                {
                    value = JsonConvert.DeserializeObject<SaveWrapper<T>>(jsonValue).v;
                }
                catch
                {
                    value = defaultValue;
                }
            }
            else
            {
                value = defaultValue;
            }
        }
        
        [Serializable]
        private struct SaveWrapper<TVal>
        {
            public TVal v;
        }
    }
}