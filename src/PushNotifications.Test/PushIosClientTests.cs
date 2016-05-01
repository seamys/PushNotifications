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
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using PushNotifications.Schema;

namespace PushNotifications.Test
{
    [TestFixture]
    public class PushIosClientTests
    {
        [SetUp]
        public void SetUp()
        {
            DeviceToken = "###";
        }

        protected string DeviceToken;

        protected PushClient GetClient()
        {
            return new PushClient("###", "###");
        }

        [TestCase("Time for individuals is consecutive and irreversible, but for the universe, just a repetitive circle。", 1)]
        [TestCase("时间对于个体来说是线性而不可逆转的；而对于整个宇宙，无非是一个周而复始的圆圈。", 2)]
        public void PushSingleDeviceAsyncTest(string alert, int badge)
        {
            var client = GetClient();
            var result = client.PushSingleDeviceAsync(DeviceToken, new PayloadNotification(alert, 1)).Result;
            Assert.NotNull(result);
        }

        protected void MapValues(IDictionary<string, object> custom, PayloadNotification notification)
        {
            string[] keys = { "action-loc-key", "category", "loc-args", "loc-key", "badge" };
            if (custom != null && custom.Count > 0)
            {
                if (custom.ContainsKey("badge"))
                {
                    var badge = custom["badge"]?.ToString();
                    int i;
                    if (int.TryParse(badge ?? "1", out i))
                        notification.Badge = i;
                }

                if (custom.ContainsKey("action-loc-key"))
                    notification.Alert.ActionLocalizedKey = custom["action-loc-key"].ToString();
                if (custom.ContainsKey("loc-args"))
                {
                    var array = custom["loc-args"] as IEnumerable;
                    if (array != null)
                    {
                        foreach (var arg in array)
                        {
                            notification.Alert.LocalizedArgs.Add(arg);
                        }
                    }
                }
                if (custom.ContainsKey("loc-key"))
                    notification.Alert.LocalizedKey = custom["loc-key"].ToString();
                foreach (var keyValuePair in custom)
                {
                    if (!keys.Contains(keyValuePair.Key))
                        notification.AddCustom(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        public void PushSingleAccountAsyncTest(string alert, int badge)
        {
            var client = GetClient();
            var result = client.PushSingleAccountAsync("13367241961", new PayloadNotification(alert, 1)).Result;
            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void PushDeviceAllParams()
        {
            var client = GetClient();

            #region 定义信息

            var custom = @"{
                'badge': 1,
                'category': '~~~',
                'action-loc-key': 'KEY',
                'loc-args': [
                    'A',
                    'B',
                    'C'
                ],
                'loc-key': '445'
            }";

            #endregion

            var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(custom);
            var notification = new PayloadNotification("测试消息");

            MapValues(obj, notification);
            var result = client.PushSingleDeviceAsync(DeviceToken, notification).Result;
        }
    }
}