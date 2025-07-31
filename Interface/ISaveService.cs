using IceCold.Interface;

namespace IceCold.SaveService.Interface
{
    public interface ISaveService : IIceColdService
    {
        public IProperty<T> GetProperty<T>(string key, T defaultValue);
        public void SaveAll();
    }
}