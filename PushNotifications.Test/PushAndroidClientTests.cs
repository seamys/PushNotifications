using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
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
            message.AddExtend("builder_id", 0);
            message.AddExtend("vibrate", 0);
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
            message.AddExtend("builder_id", 0);
            message.AddExtend("vibrate", 0);
            message.MessageType = MessageType.Notification;
            var result = client.PushMultiDeviceAsync(new List<string>() { DeviceToken }, message).Result;
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
