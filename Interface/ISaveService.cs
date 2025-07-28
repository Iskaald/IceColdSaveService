using IceCold.Interface;

namespace IceCold.SaveService.Interface
{
    public interface ISaveService : ICoreService
    {
        public IProperty<T> GetProperty<T>(string key, T defaultValue);
        public void SaveAll();
    }
}