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
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PushNotifications
{
    /// <summary>
    ///     系统工具方法
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// 格式化标签
        /// </summary>
        /// <param name="tags">标签设备分组</param>
        /// <returns>格式化后的json 字符串</returns>
        public static string ToTagParams(Dictionary<string, List<string>> tags)
        {
            var list = new List<List<string>>();

            foreach (var item in tags)
            {
                if (string.IsNullOrWhiteSpace(item.Key) || item.Value == null || item.Value.Count == 0)
                {
                    continue;
                }
                var sub = new List<string> {item.Key};
                item.Value.ForEach(x => sub.Add(x));
                list.Add(sub);
            }
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>MD5 字符串</returns>
        public static string Md5(string source)
        {
            var provider = new MD5CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(source);
            var builder = new StringBuilder();
            bytes = provider.ComputeHash(bytes);
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2").ToLower());
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取批量推送 PushId
        /// </summary>
        /// <param name="content">响应内容</param>
        /// <returns>字符串 PushId</returns>
        public static string GetPushId(string content)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                var token = JToken.Parse(content);
                return token["result"]["push_id"].Value<string>();
            }
            return null;
        }
    }
}