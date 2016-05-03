#region License

// The MIT License (MIT)
// 
// Copyright (c) 2016 seamys
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PushNotifications.Schema
{
    /// <summary>
    ///     Ios  Payload
    /// </summary>
    public class PayloadNotification : Notification
    {
        /// <summary>
        /// 消息体
        /// </summary>
        /// <param name="alert">消息体内容</param>
        public PayloadNotification(string alert)
        {
            Alert = new Alert { Body = alert };
            CustomItems = new Dictionary<string, object>();
        }

        /// <summary>
        /// 消息体
        /// </summary>
        /// <param name="alert">消息体内容</param>
        /// <param name="badge">角标</param>
        public PayloadNotification(string alert, int badge)
        {
            Alert = new Alert { Body = alert };
            Badge = badge;
            CustomItems = new Dictionary<string, object>();
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="alert">消息体</param>
        /// <param name="badge">应用角标</param>
        /// <param name="sound">通知声音</param>
        public PayloadNotification(string alert, int badge, string sound)
        {
            Alert = new Alert { Body = alert };
            Badge = badge;
            Sound = sound;
            CustomItems = new Dictionary<string, object>();
        }

        /// <summary>
        /// 消息体
        /// </summary>
        public Alert Alert { get; set; }

        /// <summary>
        /// 角标数
        /// </summary>
        public int? Badge { get; set; }

        /// <summary>
        /// 通知声音
        /// </summary>
        public string Sound { get; set; }

        /// <summary>
        /// PayloadId
        /// </summary>
        internal int PayloadId { get; set; }

        /// <summary>
        /// JSON格式字符串
        /// </summary>
        /// <returns>JSON字符串</returns>
        public override string ToJson()
        {
            var json = new JObject();
            var aps = new JObject();
            if (!Alert.IsEmpty)
            {
                if (!string.IsNullOrEmpty(Alert.Body)
                    && string.IsNullOrEmpty(Alert.LocalizedKey)
                    && string.IsNullOrEmpty(Alert.ActionLocalizedKey)
                    && (Alert.LocalizedArgs == null || Alert.LocalizedArgs.Count <= 0))
                {
                    aps["alert"] = new JValue(Alert.Body);
                }
                else
                {
                    var jsonAlert = new JObject();
                    if (!string.IsNullOrEmpty(Alert.LocalizedKey))
                    {
                        jsonAlert["loc-key"] = new JValue(Alert.LocalizedKey);
                    }

                    if (Alert.LocalizedArgs != null && Alert.LocalizedArgs.Count > 0)
                    {
                        jsonAlert["loc-args"] = new JArray(Alert.LocalizedArgs.ToArray());
                    }

                    if (!string.IsNullOrEmpty(Alert.Body))
                    {
                        jsonAlert["body"] = new JValue(Alert.Body);
                    }

                    if (!string.IsNullOrEmpty(Alert.ActionLocalizedKey))
                    {
                        jsonAlert["action-loc-key"] = new JValue(Alert.ActionLocalizedKey);
                    }
                    aps["alert"] = jsonAlert;
                }
            }
            if (Badge.HasValue)
            {
                aps["badge"] = new JValue(Badge.Value);
            }
            if (!string.IsNullOrEmpty(Sound))
            {
                aps["sound"] = new JValue(Sound);
            }
            json["aps"] = aps;
            foreach (var kv in CustomItems)
            {
                if (kv.Value != null)
                {
                    json[kv.Key] = JToken.FromObject(kv.Value);
                }
            }
            return json.ToString(Formatting.None, null);
        }
    }
}