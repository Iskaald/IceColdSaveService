using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IceCold.Interface;
using IceCold.SaveService.Interface;

namespace IceCold.SaveService
{
    [ServicePriority(3)]
    public class SaveService : ISaveService
    {
        private SaveServiceConfig config;
        private ISaveMethod saveMethod;
        
        private readonly Dictionary<string, IProperty> properties = new();

        private CancellationTokenSource cts;
        private Task saveLoopTask;
        
        public bool IsInitialized { get; private set; }
        
        public void Initialize()
        {
            config = ConfigLoader.GetConfig<SaveServiceConfig>(SaveServiceConfig.ConfigKey);
            saveMethod = config?.saveMethod;
            IsInitialized = config != null && saveMethod != null;

            if (config?.saveStrategy == SaveStrategy.Interval && saveLoopTask == null)
                saveLoopTask = SaveAllAsync();
        }

        public void Deinitialize()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
            
            IsInitialized = false;
            config = null;
            properties.Clear();
        }
        
        public IProperty<T> GetProperty<T>(string key, T defaultValue)
        {
            if (properties.TryGetValue(key, out var existing))
            {
                AttemptSaveProperty(existing);
                return (Property<T>)existing;
            }

            var prop = new Property<T>(key, defaultValue, saveMethod);
            properties[key] = prop;
            AttemptSaveProperty(prop);
            return prop;
        }

        public void SaveAll()
        {
            foreach (var prop in properties.Values)
            {
                prop.Save();
                IceColdLogger.Log($"Saved property {prop.Key}:{prop.Value}");
            }
        }

        private void AttemptSaveProperty(IProperty property)
        {
            if (config.saveStrategy != SaveStrategy.Aggressive) return;
            property.Save();
        }

        public bool OnWillQuit()
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
            
            if (config.saveStrategy != SaveStrategy.Manual)
            {
                SaveAll();
            }
            return true;
        }

        private async Task SaveAllAsync()
        {
            cts = new CancellationTokenSource();

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(config.saveIntervalInSeconds), cts.Token);
                    SaveAll();
                }
            }
            catch (TaskCanceledException)
            {
                //do nothing
            }
            finally
            {
                saveLoopTask = null;
            }
        }
    }
}