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
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using PushNotifications.Schema;
using RichardSzalay.MockHttp;

namespace PushNotifications.Test
{
    [TestFixture]
    public class PushAndroidClientTests
    {


        protected string DeviceToken;

        protected string AccessId { get; set; }

        protected string SecretKey { get; set; }

        protected PushClient GetClient()
        {
            return new PushClient("####", "####");
        }

        [SetUp]
        public void SetUp()
        {
            DeviceToken = "####";
        }

        [TestCase("The time",
            "Time for individuals is consecutive and irreversible, but for the universe, just a repetitive circle。")]
        public void PushSingleDeviceAsyncTest(string title, string content)
        {
            var client = GetClient();
            client.HttpCallback += Client_HttpCallback;
            var message = new AndroidNotification(title, content);
            message.AddCustom("builder_id", 0);
            message.AddCustom("vibrate", 0);
            message.MessageType = MessageType.Notification;
            var result = client.PushSingleDeviceAsync(DeviceToken, message).Result;
            Assert.NotNull(result);
        }

        [TestCase("The time",
            "Time for individuals is consecutive and irreversible, but for the universe, just a repetitive circle。")]
        public void QueryDeviceCountAsyncTest(string title, string content)
        {
            var client = GetClient();
            client.HttpCallback += Client_HttpCallback;
            var result = client.QueryDeviceCountAsync().Result;
            Assert.NotNull(result);
        }

        [TestCase("The time",
            "Time for individuals is consecutive and irreversible, but for the universe, just a repetitive circle。")]
        public void PushAllDeviceAsyncTest(string title, string content)
        {
            var client = GetClient();
            client.HttpCallback += Client_HttpCallback;
            var message = new AndroidNotification(title, content);
            message.AddCustom("builder_id", 0);
            message.AddCustom("vibrate", 0);
            message.MessageType = MessageType.Notification;
            var result = client.PushAllDeviceAsync(message).Result;
            Assert.NotNull(result);
        }

        private void Client_HttpCallback(string url, Dictionary<string, string> param, string content)
        {
            //to do
        }

        public void PushSingleAccountAsyncTest(string alert, int badge)
        {
            var client = GetClient();
            var result = client.PushSingleAccountAsync("13367241961", new PayloadNotification(alert, 1)).Result;
            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void PushDeviceWithAllParams()
        {
            #region 自定义字段

            var custom = @"{ 'accept_time': [{
                'start': {
                    'hour': '13',
                    'min': '00'
                },
                'end': {
                    'hour': '14',
                    'min': '00'
                }
            },
            {
                'start': {
                    'hour': '00',
                    'min': '00'
                },
                'end': {
                    'hour': '00',
                    'min': '00'
                }
            }
        ],
        'n_id': 0,
        'builder_id': 0,
        'ring': 1,
        'ring_raw': 'ring',
        'vibrate': 1,
        'lights': 1,
        'clearable': 1,
        'icon_type': 0,
        'icon_res': 'xg',
        'style_id': 1,
        'small_icon': 'xg',
        'action': {
                'action_type  ': 1,
            'activity ': 'xxx',
            'aty_attr ': {
                    'if': 0,
                'pf': 0
            },
            'browser': {
                    'url': 'www.frllk.com',
                'confirm': 1
            },
            'intent': 'xxx'
        },
        'custom_content': {
                'key1': 'value1',
            'key2': 'value2'
        }
        }";

            #endregion

            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(custom);

            var notification = new AndroidNotification("测试标题", "测试内容");
            foreach (var item in dic)
            {
                notification.AddCustom(item.Key, item.Value);
            }
            var client = GetClient();
            client.HttpCallback += Client_HttpCallback;
            var result = client.PushSingleDeviceAsync(DeviceToken, notification).Result;
            Assert.NotNull(result);
        }

        [Test]
        public async Task CancelTimingTaskTest()
        {
            var httpHandler = new MockHttpMessageHandler();
            httpHandler.When("http://openapi.xg.qq.com/v2/push/cancel_timing_task").Respond("application/json", "{ status:0 }");
            var client = new PushClient(AccessId, SecretKey, httpHandler);
            var user = await client.CancelTimingTaskAsync("any_push_id");
        }
    }
}