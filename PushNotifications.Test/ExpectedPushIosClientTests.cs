using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PushNotifications.Schema;

namespace PushNotifications.Test
{
    [TestFixture]
    public class ExpectedPushIosClientTests
    {

        protected string DeviceToken;

        [SetUp]
        public void SetUp()
        {
            DeviceToken = "#####";
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
            Assert.AreEqual(result.RetCode, 0);
        }

        public void PushSingleAccountAsyncTest(string alert, int badge)
        {
            PushClient client = GetClient();
            var result = client.PushSingleAccountAsync("13367241961", new PayloadNotification(alert, 1)).Result;
            Assert.NotNull(result);
            Assert.AreEqual(result.RetCode, 0);
        }
    }
}
