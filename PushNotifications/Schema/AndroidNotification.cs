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
        /// 扩展Key包含信鸽推送定义字段,
        ///  accept_time, n_id, builder_id, ring,ring_raw, vibrate,
        ///  lights, clearable,icon_type,icon_res, style_id, small_icon, action
        /// 不包含 title,content,custom_content 字段
        /// </summary>
        public static readonly List<string> ExtendKeys = new List<string>()
        {
            "accept_time", "n_id", "builder_id", "ring",
            "ring_raw", "vibrate", "lights", "clearable",
            "icon_type","icon_res", "style_id", "small_icon",
            "action"
        };
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public AndroidNotification(string title, string content)
        {
            Title = title;
            Content = content;
            CustomItems = new Dictionary<string, object>();
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
            if (CustomItems != null && CustomItems.Count > 0)
            {
                JObject custom = new JObject();
                foreach (var kv in CustomItems)
                {
                    if (kv.Value != null)
                    {
                        if (ExtendKeys.Contains(kv.Key))
                            json[kv.Key] = JToken.FromObject(kv.Value);
                        else
                            custom[kv.Key] = JToken.FromObject(kv.Value);
                    }
                }
                if (custom.HasValues)
                    json["custom_content"] = custom;
            }
            return json.ToString(Newtonsoft.Json.Formatting.None, null);
        }
    }
}