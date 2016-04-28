using Newtonsoft.Json.Linq;

namespace PushNotifications.Schema
{
    public class PushResult<T>
    {
        public int State { get; set; }
        public T Result { get; set; }
    }
}
