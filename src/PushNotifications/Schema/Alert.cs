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

namespace PushNotifications.Schema
{
    /// <summary>
    ///     Notification Payload
    /// </summary>
    public class Alert
    {
        /// <see cref="Alert" />
        /// <summary>
        /// 工作方法
        /// </summary>
        public Alert()
        {
            Body = null;
            ActionLocalizedKey = null;
            LocalizedKey = null;
            LocalizedArgs = new List<object>();
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 本地化动作Key
        /// </summary>
        public string ActionLocalizedKey { get; set; }

        /// <summary>
        /// 本地化Key
        /// </summary>
        public string LocalizedKey { get; set; }

        /// <summary>
        /// 添加参数
        /// </summary>
        public List<object> LocalizedArgs { get; set; }

        /// <summary>
        /// 判断消息是否为空
        /// </summary>
        public bool IsEmpty => !(!string.IsNullOrEmpty(Body)
                                 || !string.IsNullOrEmpty(ActionLocalizedKey)
                                 || !string.IsNullOrEmpty(LocalizedKey)
                                 || (LocalizedArgs != null && LocalizedArgs.Count > 0));

        /// <summary>
        /// 添加本地化参数
        /// </summary>
        /// <param name="values">参数</param>
        public void AddLocalizedArgs(params object[] values)
        {
            LocalizedArgs.AddRange(values);
        }
    }
}