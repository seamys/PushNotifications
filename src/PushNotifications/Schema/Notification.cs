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

using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PushNotifications.Schema
{
    /// <summary>
    ///     通知基类
    /// </summary>
    public abstract class Notification
    {
        /// <summary>
        /// 自定义参数
        /// </summary>
        public Dictionary<string, object> CustomItems { get; protected set; }

        /// <summary>
        /// 推送消息类型
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// 添加自定义参数
        /// </summary>
        /// <param name="key">自定义Key</param>
        /// <param name="values">自定义值</param>
        public void AddCustom(string key, object values)
        {
            if (values != null)
            {
                if (!CustomItems.ContainsKey(key))
                {
                    CustomItems.Add(key, values);
                }
                else
                {
                    CustomItems[key] = values;
                }
            }
        }

        /// <summary>
        /// 转化成Json
        /// </summary>
        /// <returns>转化成Json 字符串</returns>
        public abstract string ToJson();

        /// <summary>
        /// 重写ToString 方法
        /// </summary>
        /// <returns>JSON 格式字符串</returns>
        public override string ToString()
        {
            return ToJson();
        }

        /// <summary>
        /// 自定义时间推送
        /// </summary>
        /// <param name="sHour">起始小时 0-23 取值</param>
        /// <param name="sMin">起始分钟  0-59 取值</param>
        /// <param name="eHour">结束小时 0-23 取值</param>
        /// <param name="eMin">结束分钟  0-59 取值</param>
        public void AddAcceptTime(uint sHour, uint sMin, uint eHour, uint eMin)
        {
            var key = "accept_time";
            sHour = sHour > 23 ? 0 : sHour;
            eHour = eHour > 23 ? 0 : eHour;
            sMin = sMin > 59 ? 0 : sMin;
            eMin = eMin > 59 ? 0 : eMin;
            if (sHour > eHour || (sHour == eHour && sMin >= eMin))
            {
                eHour = 23;
                eMin = 59;
            }
            var ls = new List<object>();
            if (CustomItems.ContainsKey(key))
            {
                var ival = CustomItems[key] as IEnumerable;
                if (ival == null)
                {
                    CustomItems.Remove(key);
                }
                if (ival != null)
                {
                    ls.AddRange(ival.Cast<object>());
                }
            }
            else
            {
                CustomItems[key] = ls;
            }
            ls.Add(new
            {
                start = new { hour = $"{sHour:D2}", min = $"{sMin:D2}" },
                end = new { hour = $"{eHour:D2}", min = $"{eMin:D2}" }
            });
            CustomItems[key] = ls;
        }

        /// <summary>
        /// 指定消息发送时间整点范围
        /// </summary>
        /// <param name="sHour">起始小时</param>
        /// <param name="eHour">结束小时</param>
        public void AddAcceptTime(uint sHour, uint eHour)
        {
            AddAcceptTime(sHour, 0, eHour, 0);
        }

        /// <summary>
        /// 确定当前内容是否有效
        /// </summary>
        /// <returns></returns>
        public abstract bool IsEmpty { get; }
    }
}