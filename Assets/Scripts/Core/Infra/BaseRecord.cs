using Newtonsoft.Json;

namespace Core
{
    public abstract class BaseRecord
    {
        public string Id => this.GetType().Name;
        public virtual int Version { get; } //TODO: migration Support

        public void Populate(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }
    }
}