using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PushNotifications.Schema
{
    /// <summary>
    /// Ios ����
    /// </summary>
    public class PayloadNotification : Notification
    {
        /// <summary>
        /// ֪ͨ��Ϣ
        /// </summary>
        public Alert Alert { get; set; }
        /// <summary>
        /// ֪ͨ����
        /// </summary>
        public int? Badge { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Sound { get; set; }
        /// <summary>
        /// PayloadId
        /// </summary>
        internal int PayloadId { get; set; }
        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="alert">��Ϣ��</param>
        public PayloadNotification(string alert)
        {
            Alert = new Alert() { Body = alert };
            CustomItems = new Dictionary<string, object>();
        }
        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="alert">��Ϣ��</param>
        /// <param name="badge">δ������</param>
        public PayloadNotification(string alert, int badge)
        {
            Alert = new Alert() { Body = alert };
            Badge = badge;
            CustomItems = new Dictionary<string, object>();
        }
        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="alert">��Ϣ��</param>
        /// <param name="badge">δ������</param>
        /// <param name="sound">����</param>
        public PayloadNotification(string alert, int badge, string sound)
        {
            Alert = new Alert() { Body = alert };
            Badge = badge;
            Sound = sound;
            CustomItems = new Dictionary<string, object>();
        }
        /// <summary>
        /// ת��string ����
        /// </summary>
        /// <returns></returns>
        public override string ToJson()
        {
            JObject json = new JObject();
            JObject aps = new JObject();
            if (!Alert.IsEmpty)
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

                    if (Alert.LocalizedArgs != null && this.Alert.LocalizedArgs.Count > 0)
                        jsonAlert["loc-args"] = new JArray(this.Alert.LocalizedArgs.ToArray());

                    if (!string.IsNullOrEmpty(this.Alert.Body))
                        jsonAlert["body"] = new JValue(this.Alert.Body);

                    if (!string.IsNullOrEmpty(this.Alert.ActionLocalizedKey))
                        jsonAlert["action-loc-key"] = new JValue(this.Alert.ActionLocalizedKey);
                    aps["alert"] = jsonAlert;
                }
            }
            if (Badge.HasValue)
                aps["badge"] = new JValue(this.Badge.Value);
            if (!string.IsNullOrEmpty(this.Sound))
                aps["sound"] = new JValue(this.Sound);
            json["aps"] = aps;
            foreach (var kv in CustomItems)
            {
                if (kv.Value != null)
                    json[kv.Key] = JToken.FromObject(kv.Value);
            }
            return json.ToString(Newtonsoft.Json.Formatting.None, null);
        }
    }
}
