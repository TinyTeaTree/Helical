using System;
using Newtonsoft.Json;

namespace Game
{
    [JsonConverter(typeof(AvatarIdConverter))]
    public class AvatarId
    {
        public static readonly AvatarId Empty = "";

        private string Id;

        public override string ToString()
        {
            return Id;
        }

        // Implicit conversion from string to AvatarId
        public static implicit operator AvatarId(string id)
        {
            return new AvatarId { Id = id };
        }

        // Override Equals to compare AvatarId instances based on the Id string
        public override bool Equals(object obj)
        {
            if (obj is AvatarId otherAvatarId)
            {
                return string.Equals(Id, otherAvatarId.Id);
            }

            return false;
        }

        // Override GetHashCode to ensure consistent hashing
        public override int GetHashCode()
        {
            return Id != null ? Id.GetHashCode() : 0;
        }

        // Equality operator
        public static bool operator ==(AvatarId a, AvatarId b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return string.Equals(a.Id, b.Id);
        }

        // Inequality operator
        public static bool operator !=(AvatarId a, AvatarId b)
        {
            return !(a == b);
        }
    }


    public class AvatarIdConverter : JsonConverter<AvatarId>
    {
        // This tells Newtonsoft to handle AvatarId as a string
        public override void WriteJson(JsonWriter writer, AvatarId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString()); // Serialize AvatarId as a string
        }

        public override AvatarId ReadJson(JsonReader reader, Type objectType, AvatarId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Deserialize the string into an AvatarId
            return reader.TokenType == JsonToken.String ? (string)reader.Value : null;
        }
    }
}