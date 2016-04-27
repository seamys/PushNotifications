using System.Collections.Generic;

namespace PushNotifications.Schema
{
    public abstract class Notification
    {
        ////表示消息将在哪些时间段允许推送给用户，选填
        public List<AcceptTime> AcceptTime { get; set; }
        public Dictionary<string, object[]> CustomItems
        {
            get;
            protected set;
        }
        public void AddCustom(string key, params object[] values)
        {
            if (values != null)
                this.CustomItems.Add(key, values);
        }
        public abstract string ToJson();
    }
}