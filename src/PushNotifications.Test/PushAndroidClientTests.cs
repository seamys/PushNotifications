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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        protected static string DeviceToken;

        protected string AccessId { get; set; }

        protected string SecretKey { get; set; }

        protected PushClient GetClient()
        {
            return new PushClient("####", "####");
        }

        protected PushClient GetClient(string url, List<KeyValuePair<string, string>> kvs, object content)
        {
            var httpHandler = new MockHttpMessageHandler();
            MockedRequest request = httpHandler.When(url);
            var list = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("access_id", AccessId),
                new KeyValuePair<string, string>("timestamp", "1462278512"),
                new KeyValuePair<string, string>("valid_time", "600")
            };
            list.AddRange(kvs);

            var builder = new StringBuilder();
            builder.Append("POST");
            builder.Append(url.Replace("http://", string.Empty));

            foreach (var item in list.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                builder.Append($"{item.Key}={item.Value}");
            }
            builder.Append(SecretKey);

            string signature = Utils.Md5(builder.ToString());

            list.Add(new KeyValuePair<string, string>("sign", signature));

            request.WithFormData(list);

            request.Respond("application/json", JsonConvert.SerializeObject(content));

            return new PushClient(AccessId, SecretKey)
            {
                Timestamp = 1462278512,
                ValidTime = 600,
                HttpHandler = httpHandler
            };
        }

        [SetUp]
        public void SetUp()
        {
            DeviceToken = "DeviceToken-abcdefg";
            AccessId = "AccessId-123";
            SecretKey = "SecretKey-ABC";
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

        [TestCase("http://openapi.xg.qq.com/v2/tags/query_app_tags", 0u, 100u)]
        [TestCase("http://openapi.xg.qq.com/v2/tags/query_app_tags", 2u, 101u)]
        public void QueryTagsAsyncTest(string url, uint start, uint limit)
        {
            var kvs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("start",start.ToString()),
                new KeyValuePair<string, string>("limit",(limit>100?100:limit).ToString())
            };
            PushClient client = GetClient(url, kvs, new { total = 0, tags = new[] { "tag1", "tag2" } });
            var user = client.QueryTagsAsync(start, limit).Result;
            Assert.NotNull(user);
        }

        [TestCase("http://openapi.xg.qq.com/v2/tags/query_token_tags", null)]
        [TestCase("http://openapi.xg.qq.com/v2/tags/query_token_tags", "deviceToken-abc")]
        public void QueryTokenTagsAsyncTest(string url, string deviceToken)
        {
            var kvs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("device_token",deviceToken)
            };
            PushClient client = GetClient(url, kvs, new { tags = new[] { "tag1", "tag2" } });
            if (!string.IsNullOrWhiteSpace(deviceToken))
            {
                var user = client.QueryTokenTagsAsync(deviceToken).Result;
                Assert.NotNull(user);
            }
            else
            {
                Assert.Catch<ArgumentException>(() => { client.QueryTokenTagsAsync(deviceToken).Wait(); });
            }

        }

        [TestCase("http://openapi.xg.qq.com/v2/tags/query_tag_token_num", " ")]
        [TestCase("http://openapi.xg.qq.com/v2/tags/query_tag_token_num", "tag1")]
        public void QueryTagsTokenAsyncTest(string url, string tag)
        {
            var kvs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("tag", tag)
            };
            PushClient client = GetClient(url, kvs, new { device_num = 123456 });
            if (!string.IsNullOrWhiteSpace(tag))
            {
                var user = client.QueryTagsTokenAsync(tag).Result;
                Assert.NotNull(user);
            }
            else
            {
                Assert.Catch<ArgumentException>(() => { client.QueryTagsTokenAsync(tag).Wait(); });
            }
        }

        [TestCase("http://openapi.xg.qq.com/v2/push/delete_offline_msg", " ")]
        [TestCase("http://openapi.xg.qq.com/v2/push/delete_offline_msg", "any_push_id")]
        public void DeleteOfflineTest(string url, string pushId)
        {
            var kvs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("push_id", pushId)
            };
            PushClient client = GetClient(url, kvs, new { status = 0 });
            if (!string.IsNullOrWhiteSpace(pushId))
            {
                var user = client.DeleteOfflineAsync(pushId).Result;
                Assert.NotNull(user);
            }
            else
            {
                Assert.Catch<ArgumentException>(() => { client.DeleteOfflineAsync(pushId).Wait(); });
            }
        }

        [TestCase("http://openapi.xg.qq.com/v2/push/cancel_timing_task", null)]
        [TestCase("http://openapi.xg.qq.com/v2/push/cancel_timing_task", "any_push_id")]
        public void CancelTimingTaskTest(string url, string pushId)
        {
            var kvs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("push_id", pushId)
            };
            PushClient client = GetClient(url, kvs, new { status = 0 });
            if (!string.IsNullOrWhiteSpace(pushId))
            {
                var user = client.CancelTimingTaskAsync(pushId).Result;
                Assert.NotNull(user);
            }
            else
            {
                Assert.Catch<ArgumentException>(() => { client.CancelTimingTaskAsync(pushId).Wait(); });
            }
        }
    }
}