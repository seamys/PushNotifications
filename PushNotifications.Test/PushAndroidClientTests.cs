using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using PushNotifications.Schema;

namespace PushNotifications.Test
{
    [TestFixture]
    public class PushAndroidClientTests
    {

        protected string DeviceToken;

        [SetUp]
        public void SetUp()
        {
            DeviceToken = "920dcb46482cbbdd43afabff0f011df3ea15cb8a";
        }

        protected PushClient GetClient()
        {
            return new PushClient("2100194555", "606a7632f3dbbf9025632a0c0fb2ca0d");
        }

        [TestCase("The time", "Time for individuals is consecutive and irreversible, but for the universe, just a repetitive circle。")]
        public void PushSingleDeviceAsyncTest(string title, string content)
        {
            PushClient client = GetClient();
            client.HttpCallback += Client_HttpCallback;
            var message = new AndroidNotification(title, content);
            message.AddCustom("builder_id", 0);
            message.AddCustom("vibrate", 0);
            message.MessageType = MessageType.Notification;
            var result = client.PushSingleDeviceAsync(DeviceToken, message).Result;
            Assert.NotNull(result);
        }
        [TestCase("The time", "Time for individuals is consecutive and irreversible, but for the universe, just a repetitive circle。")]
        public void QueryDeviceCountAsyncTest(string title, string content)
        {
            PushClient client = GetClient();
            client.HttpCallback += Client_HttpCallback;
            var result = client.QueryDeviceCountAsync().Result;
            Assert.NotNull(result);
        }
        [TestCase("The time", "Time for individuals is consecutive and irreversible, but for the universe, just a repetitive circle。")]
        public void PushAllDeviceAsyncTest(string title, string content)
        {
            PushClient client = GetClient();
            client.HttpCallback += Client_HttpCallback;
            var message = new AndroidNotification(title, content);
            message.AddCustom("builder_id", 0);
            message.AddCustom("vibrate", 0);
            message.MessageType = MessageType.Notification;
            var result = client.PushMultiDeviceAsync(new List<string>() { DeviceToken }, message).Result;
            Assert.NotNull(result);
        }
        [Test]
        public void PushDeviceWithAllParams()
        {
            #region 自定义字段
            string custom = @"{ 'accept_time': [{
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

            Dictionary<string, Object> dic = JsonConvert.DeserializeObject<Dictionary<string, Object>>(custom);

            AndroidNotification notification = new AndroidNotification("测试标题", "测试内容");
            foreach (var item in dic)
            {
                notification.AddCustom(item.Key, item.Value);
            }
            PushClient client = GetClient();
            client.HttpCallback += Client_HttpCallback;
            var result = client.PushSingleDeviceAsync(DeviceToken, notification).Result;
            Assert.NotNull(result);
        }

        private void Client_HttpCallback(string url, Dictionary<string, string> param, string content)
        {
            //to do
        }

        public void PushSingleAccountAsyncTest(string alert, int badge)
        {
            PushClient client = GetClient();
            var result = client.PushSingleAccountAsync("13367241961", new PayloadNotification(alert, 1)).Result;
            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }
    }
}
