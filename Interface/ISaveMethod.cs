namespace IceCold.SaveService.Interface
{
    public interface ISaveMethod
    {
        public void SaveProperty(string key, string jsonValue);

        public bool Exists(string key, out string value);
    }
}