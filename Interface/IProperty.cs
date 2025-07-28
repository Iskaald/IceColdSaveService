namespace IceCold.SaveService.Interface
{
    public interface IProperty
    {
        public object Value { get; set; }
        public string Key { get; }
        
        public void Save();
    }
    
    public interface IProperty<T> : IProperty
    {
        public new T Value { get; set; }
    }
}