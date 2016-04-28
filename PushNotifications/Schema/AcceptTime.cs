using System;

namespace PushNotifications.Schema
{
    public class AcceptTime
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public override string ToString()
        {
            return $"{{ \"start\":{{ \"hour\":\"{Start.Hour}\"}},{{\"min\":\"{Start.Minute}\"}},\"end\":{{ \"hour\":\"{End.Hour}\"}},{{\"min\":\"{End.Minute}\"}}}}";
        }
    }
}