using System.Threading.Tasks;

namespace IceCold.SaveService.Interface
{
    public interface ISaveMethod
    {
        public Task<bool> SaveProperty(string key, string jsonValue);

        public bool Exists(string key, out string value);
    }
}