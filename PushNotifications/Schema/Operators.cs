using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PushNotifications.Schema
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Operators
    {
        AND,
        OR
    }
}