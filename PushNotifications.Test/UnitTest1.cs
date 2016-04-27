using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PushNotifications.Schema;

namespace PushNotifications.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            PushClient client = new PushClient("<accessId>", "<secretKey>");
            //全量推送
            client.PushAllDeviceAsync(new PayloadNotification("test message", 1)).Wait();
            //推送单条消息
            client.PushSingleDeviceAsync("<deviceToken>", new PayloadNotification("test message", 1));
        }
    }
}
