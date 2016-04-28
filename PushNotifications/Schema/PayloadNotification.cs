using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PushNotifications.Schema
{
    public class PayloadNotification : Notification
    {
        public Alert Alert { get; set; }

        public int? Badge { get; set; }

        public string Sound { get; set; }

        internal int PayloadId { get; set; }
        public PayloadNotification(string alert)
        {
            Alert = new Alert() { Body = alert };
            CustomItems = new Dictionary<string, object[]>();
        }

        public PayloadNotification(string alert, int badge)
        {
            Alert = new Alert() { Body = alert };
            Badge = badge;
            CustomItems = new Dictionary<string, object[]>();
        }

        public PayloadNotification(string alert, int badge, string sound)
        {
            Alert = new Alert() { Body = alert };
            Badge = badge;
            Sound = sound;
            CustomItems = new Dictionary<string, object[]>();
        }
        public override string ToJson()
        {
            JObject json = new JObject();

            JObject aps = new JObject();

            if (!this.Alert.IsEmpty)
            {
                if (!string.IsNullOrEmpty(this.Alert.Body)
                    && string.IsNullOrEmpty(this.Alert.LocalizedKey)
                    && string.IsNullOrEmpty(this.Alert.ActionLocalizedKey)
                    && (this.Alert.LocalizedArgs == null || this.Alert.LocalizedArgs.Count <= 0))
                {
                    aps["alert"] = new JValue(this.Alert.Body);
                }
                else
                {
                    JObject jsonAlert = new JObject();

                    if (!string.IsNullOrEmpty(this.Alert.LocalizedKey))
                        jsonAlert["loc-key"] = new JValue(this.Alert.LocalizedKey);

                    if (this.Alert.LocalizedArgs != null && this.Alert.LocalizedArgs.Count > 0)
                        jsonAlert["loc-args"] = new JArray(this.Alert.LocalizedArgs.ToArray());

                    if (!string.IsNullOrEmpty(this.Alert.Body))
                        jsonAlert["body"] = new JValue(this.Alert.Body);

                    if (!string.IsNullOrEmpty(this.Alert.ActionLocalizedKey))
                        jsonAlert["action-loc-key"] = new JValue(this.Alert.ActionLocalizedKey);

                    aps["alert"] = jsonAlert;
                }
            }

            if (this.Badge.HasValue)
                aps["badge"] = new JValue(this.Badge.Value);

            if (!string.IsNullOrEmpty(this.Sound))
                aps["sound"] = new JValue(this.Sound);
            json["aps"] = aps;
            if (AcceptTime != null && AcceptTime.Count > 0)
                json["accept_time"] = new JArray(AcceptTime);
            foreach (string key in this.CustomItems.Keys)
            {
                if (this.CustomItems[key].Length == 1)
                    json[key] = new JValue(this.CustomItems[key][0]);
                else if (this.CustomItems[key].Length > 1)
                    json[key] = new JArray(this.CustomItems[key]);
            }

            string rawString = json.ToString(Newtonsoft.Json.Formatting.None, null);

            StringBuilder encodedString = new StringBuilder();
            foreach (char c in rawString)
            {
                if ((int)c < 32 || (int)c > 127)
                    encodedString.Append($"\\u{Convert.ToUInt32(c):x4}");
                else
                    encodedString.Append(c);
            }
            return rawString;// encodedString.ToString();
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}
