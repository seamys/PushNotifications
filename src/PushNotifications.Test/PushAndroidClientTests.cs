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
using System.Security.Cryptography.X509Certificates;
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
            MockHttpMessageHandler httpHandler = new MockHttpMessageHandler();
            InitClient(url, kvs, content, ref httpHandler);
            return new PushClient(AccessId, SecretKey)
            {
                Timestamp = 1462278512,
                ValidTime = 600,
                HttpHandler = httpHandler
            };
        }

        protected void InitClient(string url, List<KeyValuePair<string, string>> kvs, object content, ref MockHttpMessageHandler httpHandler)
        {
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
        }

        public object GetResult(object result)
        {
            return new { ret_code = 0, err_msg = "ok", result };
        }

        [SetUp]
        public void SetUp()
        {
            DeviceToken = "DeviceToken-abcdefg";
            AccessId = "AccessId-123";
            SecretKey = "SecretKey-ABC";
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
            // client.HttpCallback += Client_HttpCallback;
            var result = client.PushSingleDeviceAsync(DeviceToken, notification).Result;
            Assert.NotNull(result);
        }

        [TestCase("device-token-abc", "message title", "message content")]
        public void PushSingleDeviceAsyncTest(string deviceToken, string title, string content)
        {
            string url = "http://openapi.xg.qq.com/v2/push/single_device";
            var dic = new Dictionary<string, string>
            {
                {"device_token", deviceToken},
                {"message", "{\"title\":\"message title\",\"content\":\"message content\"}"},
                {"expire_time", "86400"},
                {"message_type", "1"},
                {"multi_pkg", "1"}
            };
            var client = GetClient(url, dic.ToList(), GetResult(null));
            var result = client.PushSingleDeviceAsync(deviceToken, new AndroidNotification(title, content)).Result;
            Assert.NotNull(result);
        }

        [TestCase("message title", "message content")]
        public void CreateMultiPush(string title, string content)
        {
            string url = "http://openapi.xg.qq.com/v2/push/create_multipush";
            var dic = new Dictionary<string, string>
            {
                {"message", "{\"title\":\"message title\",\"content\":\"message content\"}"},
                {"expire_time", "86400"},
                {"message_type", "1"},
                {"multi_pkg", "1"}
            };
            var client = GetClient(url, dic.ToList(), GetResult(new { push_id = "push_id-1" }));
            var result = client.CreateMultiPushAsync(new AndroidNotification(title, content)).Result;
            Assert.NotNull(result);
        }

        [TestCase(new[] { "device-1", "device-2" }, "push_id-1")]
        public void PushMultiDeviceAsyncTest(string[] devices, string pushId)
        {
            string url = "http://openapi.xg.qq.com/v2/push/device_list_multiple";
            var dic = new Dictionary<string, string>
            {
                { "device_list",JsonConvert.SerializeObject(devices)},
                { "push_id", pushId}
            };
            var client = GetClient(url, dic.ToList(), GetResult(new { push_id = "push_id-1" }));
            var result = client.PushMultiDeviceAsync(devices, pushId).Result;
            Assert.NotNull(result);
        }

        [TestCase(new[] { "device-1", "device-2" }, "message title", "message content")]
        public void PushMultiDeviceAsyncTest(string[] devices, string title, string content)
        {
            MockHttpMessageHandler httpHandler = new MockHttpMessageHandler();

            var dic = new Dictionary<string, string>()
            {
                {"message", "{\"title\":\"message title\",\"content\":\"message content\"}"},
                {"expire_time", "86400"},
                {"message_type", "1"},
                {"multi_pkg", "1"}
            };
            InitClient("http://openapi.xg.qq.com/v2/push/create_multipush", dic.ToList(), GetResult(new { push_id = "push_id-1" }), ref httpHandler);

            dic = new Dictionary<string, string>
             {
                { "device_list",JsonConvert.SerializeObject(devices)},
                { "push_id", "push_id-1"}
            };
            InitClient("http://openapi.xg.qq.com/v2/push/device_list_multiple", dic.ToList(), GetResult(new { push_id = "push_id-1" }), ref httpHandler);

            var client = new PushClient(AccessId, SecretKey)
            {
                Timestamp = 1462278512,
                ValidTime = 600,
                HttpHandler = httpHandler
            };

            var result = client.PushMultiDeviceAsync(devices, new AndroidNotification(title, content)).Result;
            Assert.NotNull(result);
        }

        [TestCase("message title", "message content")]
        public void PushAllDeviceAsyncTest(string title, string content)
        {
            string url = "http://openapi.xg.qq.com/v2/push/all_device";
            var dic = new Dictionary<string, string>
            {
                {"message", "{\"title\":\"message title\",\"content\":\"message content\"}"},
                {"expire_time", "86400"},
                {"message_type", "1"},
                {"multi_pkg", "1"}
            };
            var client = GetClient(url, dic.ToList(), GetResult(new { push_id = "push_id~" }));
            var result = client.PushAllDeviceAsync(new AndroidNotification(title, content)).Result;
            Assert.NotNull(result);
        }

        [TestCase("message title", "message content", 10, 10)]
        public void PushAllDeviceAsyncTest(string title, string content, int loopTimes, int loopInterval)
        {
            string url = "http://openapi.xg.qq.com/v2/push/all_device";
            var dic = new Dictionary<string, string>
            {
                {"message", "{\"title\":\"message title\",\"content\":\"message content\"}"},
                {"expire_time", "86400"},
                {"message_type", "1"},
                {"multi_pkg", "1"},
                {"loop_times",loopTimes.ToString()},
                {"loop_interval",loopInterval.ToString()}
            };
            var client = GetClient(url, dic.ToList(), GetResult(new { push_id = "push_id~" }));
            var result = client.PushAllDeviceAsync(new AndroidNotification(title, content), loopTimes, loopInterval).Result;
            Assert.NotNull(result);
        }

        [TestCase(new[] { "tag1", "tag2" }, "message title", "message content", Operators.AND, 10, 12)]
        public void PushTagDeviceAsyncTest(string[] tags, string title, string content, Operators operators, int loopTimes, int loopInterval)
        {
            string url = "http://openapi.xg.qq.com/v2/push/tags_device";
            var dic = new Dictionary<string, string>
            {
                {"message", "{\"title\":\"message title\",\"content\":\"message content\"}"},
                {"expire_time", "86400"},
                {"tags_list",JsonConvert.SerializeObject(tags)},
                {"message_type", "1"},
                {"tags_op",operators == Operators.AND?"AND":"OR"},
                {"multi_pkg", "1"},
                {"loop_times",loopTimes.ToString()},
                {"loop_interval",loopInterval.ToString()}
            };
            var client = GetClient(url, dic.ToList(), GetResult(new { push_id = "push_id-1" }));
            var result = client.PushTagsDeviceAsync(tags, new AndroidNotification(title, content), operators, loopTimes, loopInterval).Result;
            Assert.NotNull(result);
        }

        [TestCase("13367241961", "message title", "message content")]
        public void PushSingleAccountAsyncTest(string account, string title, string content)
        {
            string url = "http://openapi.xg.qq.com/v2/push/single_account";
            var dic = new Dictionary<string, string>
            {
                {"account", account},
                {"message", "{\"title\":\"message title\",\"content\":\"message content\"}"},
                {"expire_time", "86400"},
                {"message_type", "1"},
                {"multi_pkg", "1"}
            };
            var client = GetClient(url, dic.ToList(), new { ret_code = 0, err_msg = "ok" });
            var result = client.PushSingleAccountAsync(account, new AndroidNotification(title, content)).Result;
            Assert.NotNull(result);
        }

        [TestCase(new[] { "account-1", "device-2" }, "push_id-1")]
        public void PushMultiAccountAsyncTest(string[] accounts, string pushId)
        {
            string url = "http://openapi.xg.qq.com/v2/push/account_list";
            var dic = new Dictionary<string, string>
            {
                { "account_list",JsonConvert.SerializeObject(accounts)},
                { "push_id", pushId}
            };
            var client = GetClient(url, dic.ToList(), GetResult(new { push_id = "push_id-1" }));
            var result = client.PushMultiAccountAsync(accounts, pushId).Result;
            Assert.NotNull(result);
        }

        [TestCase(new[] { "account-1", "account-1" }, "message title", "message content")]
        public void PushMultiAccountAsyncTest(string[] accounts, string title, string content)
        {
            MockHttpMessageHandler httpHandler = new MockHttpMessageHandler();

            var dic = new Dictionary<string, string>()
            {
                {"message", "{\"title\":\"message title\",\"content\":\"message content\"}"},
                {"expire_time", "86400"},
                {"message_type", "1"},
                {"multi_pkg", "1"}
            };
            InitClient("http://openapi.xg.qq.com/v2/push/create_multipush", dic.ToList(), GetResult(new { push_id = "push_id-1" }), ref httpHandler);

            dic = new Dictionary<string, string>
             {
                { "account_list",JsonConvert.SerializeObject(accounts)},
                { "push_id", "push_id-1"}
            };
            InitClient("http://openapi.xg.qq.com/v2/push/account_list", dic.ToList(), GetResult(new { push_id = "push_id-1" }), ref httpHandler);

            var client = new PushClient(AccessId, SecretKey)
            {
                Timestamp = 1462278512,
                ValidTime = 600,
                HttpHandler = httpHandler
            };

            var result = client.PushMultiAccountAsync(accounts, new AndroidNotification(title, content)).Result;
            Assert.NotNull(result);
        }

        [Test]
        public void BatchSetAsyncTest()
        {
            var tagTokens = new Dictionary<string, IEnumerable<string>>()
            {
                {"tag1", new[] {"token1", "token2"}},
                {"tag2", new[] {"token3", "token2"}}
            };
            var dic = new Dictionary<string, string>()
            {
                { "tag_token_list",Utils.ToTagParams(tagTokens)}
            };
            var client = GetClient("http://openapi.xg.qq.com/v2/tags/batch_set", dic.ToList(), GetResult(null));
            var result = client.SetTagsAsync(tagTokens).Result;
            Assert.NotNull(result);
        }

        [Test]
        public void BatchDelAsyncTest()
        {
            var tagTokens = new Dictionary<string, IEnumerable<string>>()
            {
                {"tag1", new[] {"token1", "token2"}},
                {"tag2", new[] {"token3", "token2"}}
            };
            var dic = new Dictionary<string, string>()
            {
                { "tag_token_list",Utils.ToTagParams(tagTokens)}
            };
            var client = GetClient("http://openapi.xg.qq.com/v2/tags/batch_del", dic.ToList(), GetResult(null));
            var result = client.DeleteTagsAsync(tagTokens).Result;
            Assert.NotNull(result);
        }

        [TestCase("account-a", "device_token-a-b")]
        public void DelAccountTokensAsyncTest(string account, string deviceToken)
        {
            var dic = new Dictionary<string, string>()
            {
                { "account",account},
                { "device_token",deviceToken}
            };
            var client = GetClient("http://openapi.xg.qq.com/v2/application/del_app_account_tokens", dic.ToList(), GetResult(new { tokens = new[] { "token-a-a", "token-a-c" } }));
            var result = client.DeleteAccountTokensAsync(account, deviceToken).Result;
            Assert.NotNull(result);
        }

        [TestCase("account-a")]
        public void DelAccountAllTokensAsyncTest(string account)
        {
            var dic = new Dictionary<string, string>()
            {
                { "account",account}
            };
            var client = GetClient("http://openapi.xg.qq.com/v2/application/del_app_account_all_tokens", dic.ToList(), GetResult(null));
            var result = client.DeleteAccountTokensAsync(account).Result;
            Assert.NotNull(result);
        }
        [Test]
        public void QueryMsgStatus()
        {
            var pushIds = new[] { "push-1", "push-2" };
            var dic = new Dictionary<string, string>()
            {
                { "push_id",JsonConvert.SerializeObject(pushIds.Select(x=>new {push_id=x}))}
            };
            var client = GetClient("http://openapi.xg.qq.com/v2/push/get_msg_status", dic.ToList(), GetResult(new
            {
                list = new List<object>() { new
                {
                    push_id = "push-1",
                    status = 2,
                    start_time = "2016-05-04 22:40:44"
                }
                }
            }));
            var result = client.QueryPushStatusAsync(pushIds).Result;
            Assert.NotNull(result);
        }

        [TestCase(0u, 100u)]
        [TestCase(2u, 101u)]
        public void QueryTagsAsyncTest(uint start, uint limit)
        {
            string url = "http://openapi.xg.qq.com/v2/tags/query_app_tags";
            var kvs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("start",start.ToString()),
                new KeyValuePair<string, string>("limit",(limit>100?100:limit).ToString())
            };
            PushClient client = GetClient(url, kvs, new { total = 0, tags = new[] { "tag1", "tag2" } });
            var user = client.QueryTagsAsync(start, limit).Result;
            Assert.NotNull(user);
        }

        [TestCase(null)]
        [TestCase("deviceToken-abc")]
        public void QueryTokenTagsAsyncTest(string deviceToken)
        {
            string url = "http://openapi.xg.qq.com/v2/tags/query_token_tags";
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

        [TestCase(" ")]
        [TestCase("tag1")]
        public void QueryTagsTokenAsyncTest(string tag)
        {
            string url = "http://openapi.xg.qq.com/v2/tags/query_tag_token_num";
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

        [TestCase(" ")]
        [TestCase("any_push_id")]
        public void DeleteOfflineTest(string pushId)
        {
            string url = "http://openapi.xg.qq.com/v2/push/delete_offline_msg";
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

        [TestCase(null)]
        [TestCase("any_push_id")]
        public void CancelTimingTaskTest(string pushId)
        {
            string url = "http://openapi.xg.qq.com/v2/push/cancel_timing_task";
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