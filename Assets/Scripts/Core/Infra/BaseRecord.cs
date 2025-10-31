using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core
{
    public abstract class BaseRecord
    {
        public string Id => this.GetType().Name;
        public virtual int Version { get; set; } //TODO: migration Support

        public void Populate(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }

        public void Populate(JObject o)
        {
            using var jsonReader = o.CreateReader();
            JsonSerializer.Create().Populate(jsonReader, this);
        }
    }
}