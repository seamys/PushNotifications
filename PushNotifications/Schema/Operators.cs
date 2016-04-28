using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PushNotifications.Schema
{
    /// <summary>
    /// 逻辑操作
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Operators
    {
        /// <summary>
        /// AND 操作
        /// </summary>
        AND,
        /// <summary>
        /// OR 操作
        /// </summary>
        OR
    }
}