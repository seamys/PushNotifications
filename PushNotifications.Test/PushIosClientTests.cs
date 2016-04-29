using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using PushNotifications.Schema;

namespace PushNotifications.Test
{
    [TestFixture]
    public class PushIosClientTests
    {

        protected string DeviceToken;

        [SetUp]
        public void SetUp()
        {
            DeviceToken = "###";
        }

        protected PushClient GetClient()
        {
            return new PushClient("###", "###");
        }

        [TestCase("Time for individuals is consecutive and irreversible, but for the universe, just a repetitive circle。", 1)]
        [TestCase("时间对于个体来说是线性而不可逆转的；而对于整个宇宙，无非是一个周而复始的圆圈。", 2)]
        public void PushSingleDeviceAsyncTest(string alert, int badge)
        {
            PushClient client = GetClient();
            var result = client.PushSingleDeviceAsync(DeviceToken, new PayloadNotification(alert, 1)).Result;
            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }
        [Test]
        public void PushDeviceAllParams()
        {
            PushClient client = GetClient();
            #region 定义信息
            string custom = @"{
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

            Dictionary<string, Object> obj = JsonConvert.DeserializeObject<Dictionary<string, Object>>(custom);
            PayloadNotification notification = new PayloadNotification("测试消息");
           
            MapValues(obj, notification);
            var result = client.PushSingleDeviceAsync(DeviceToken, notification).Result;
        }

        protected void MapValues(IDictionary<string, object> custom, PayloadNotification notification)
        {
            string[] keys = { "action-loc-key", "category", "loc-args", "loc-key", "badge" };
            if (custom != null && custom.Count > 0)
            {
                if (custom.ContainsKey("badge"))
                {
                    string badge = custom["badge"]?.ToString();
                    int i;
                    if (int.TryParse(badge ?? "1", out i))
                        notification.Badge = i;
                }

                if (custom.ContainsKey("action-loc-key"))
                    notification.Alert.ActionLocalizedKey = custom["action-loc-key"].ToString();
                if (custom.ContainsKey("loc-args"))
                {
                    IEnumerable array = custom["loc-args"] as IEnumerable;
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
            PushClient client = GetClient();
            var result = client.PushSingleAccountAsync("13367241961", new PayloadNotification(alert, 1)).Result;
            Assert.NotNull(result);
            Assert.AreEqual(result, 0);
        }
    }
}
