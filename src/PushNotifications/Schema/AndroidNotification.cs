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
    ///     Android 通知
    /// </summary>
    public class AndroidNotification : Notification
    {
        /// <summary>
        /// 扩展Key包含信鸽推送定义字段,
        /// accept_time, n_id, builder_id, ring,ring_raw, vibrate,
        /// lights, clearable,icon_type,icon_res, style_id, small_icon, action
        /// 不包含 title,content,custom_content 字段
        /// </summary>
        public static readonly List<string> ExtendKeys = new List<string>
        {
            "accept_time",
            "n_id",
            "builder_id",
            "ring",
            "ring_raw",
            "vibrate",
            "lights",
            "clearable",
            "icon_type",
            "icon_res",
            "style_id",
            "small_icon",
            "action"
        };

        /// <see cref="AndroidNotification" />
        /// <summary>
        /// 无参构造方法
        /// </summary>
        public AndroidNotification()
        {
            CustomItems = new Dictionary<string, object>();
        }

        /// <see cref="AndroidNotification" />
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="content">消息内容</param>
        public AndroidNotification(string title, string content)
        {
            Title = title;
            Content = content;
            CustomItems = new Dictionary<string, object>();
        }

        /// <summary>
        /// 判断内容是否为空
        /// </summary>
        /// <returns>是否为空</returns>
        public override bool IsEmpty => (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content));

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
        /// <returns>转换JSON 字符串</returns>
        public override string ToJson()
        {
            var json = new JObject
            {
                ["title"] = Title,
                ["content"] = Content
            };
            if (CustomItems != null && CustomItems.Count > 0)
            {
                var custom = new JObject();
                foreach (var kv in CustomItems)
                {
                    if (kv.Value != null)
                    {
                        if (ExtendKeys.Contains(kv.Key))
                        {
                            json[kv.Key] = JToken.FromObject(kv.Value);
                        }
                        else
                        {
                            custom[kv.Key] = JToken.FromObject(kv.Value);
                        }
                    }
                }
                if (custom.HasValues)
                {
                    json["custom_content"] = custom;
                }
            }
            return json.ToString(Formatting.None, null);
        }


    }
}