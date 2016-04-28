using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PushNotifications.Schema
{
    /// <summary>
    /// Android 通知
    /// </summary>
    public class AndroidNotification : Notification
    {

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public AndroidNotification(string title, string content)
        {
            Title = title;
            Content = content;
            ExtendItems = new Dictionary<string, object>();
        }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public Dictionary<string, object> ExtendItems { get; set; }
        /// <summary>
        /// 添加扩展字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public void AddExtend(string key, object values)
        {
            if (values != null)
                this.ExtendItems.Add(key, values);
        }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 动作，选填。默认为打开app
        /// </summary>
        public Action Action { get; set; }
        /// <summary>
        /// 转化string 方法
        /// </summary>
        /// <returns></returns>
        public override string ToJson()
        {

            JObject json = new JObject
            {
                ["title"] = Title,
                ["content"] = Content,
            };
            if (Action != null)
            {
                json["action"] = new JObject(Action);
            }
            if (CustomItems != null && CustomItems.Count > 0)
            {
                JObject custom = new JObject();
                foreach (string key in this.CustomItems.Keys)
                {
                    if (CustomItems[key].Length == 1)
                        custom[key] = new JValue(this.CustomItems[key][0]);
                    else if (CustomItems[key].Length > 1)
                        custom[key] = new JArray(this.CustomItems[key]);
                }
                json["custom_content"] = new JArray(custom);
            }
            if (ExtendItems.Count > 0)
            {
                foreach (var item in ExtendItems)
                {
                    if (item.Value != null)
                        json[item.Key] = new JValue(item.Value);
                }
            }
            if (AcceptTime != null)
                json["accept_time"] = new JArray(AcceptTime);
            string rawString = json.ToString(Newtonsoft.Json.Formatting.None, null);
            StringBuilder encodedString = new StringBuilder();
            foreach (char c in rawString)
            {
                if (c < 32 || c > 127)
                    encodedString.Append($"\\u{Convert.ToUInt32(c):x4}");
                else
                    encodedString.Append(c);
            }
            return rawString;// encodedString.ToString();
        }
    }
}