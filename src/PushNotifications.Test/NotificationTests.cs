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
using Newtonsoft.Json;
using NUnit.Framework;
using PushNotifications.Schema;

namespace PushNotifications.Test
{
    [TestFixture]
    public class NotificationAddAcceptTimeTests
    {
        [TestCase(20u, 0u, 23u, 0u)]
        public void TestAddAcceptTime(uint sHour, uint sMin, uint eHour, uint eMin)
        {
            var notification = new AndroidNotification("title", "content");
            notification.AddAcceptTime(20, 23);
            var time = JsonConvert.SerializeObject(notification.CustomItems["accept_time"]);
            Assert.NotNull(time);
            Assert.AreEqual(time,
                "[{\"start\":{\"hour\":\"20\",\"min\":\"00\"},\"end\":{\"hour\":\"23\",\"min\":\"00\"}}]");
            notification.CustomItems.Remove("accept_time");
        }

        [TestCase(10u, 11u)]
        public void TestKeyAlreadyExists(uint sHour, uint eHour)
        {
            var notification = new AndroidNotification("message", "content");
            notification.AddCustom("accept_time",
                new List<object> { new { start = new { hour = "10", min = "10" }, end = new { hour = "11", min = "10" } } });
            notification.AddAcceptTime(sHour, eHour);
            var time = JsonConvert.SerializeObject(notification.CustomItems["accept_time"]);
            Assert.AreEqual(time,
                "[{\"start\":{\"hour\":\"10\",\"min\":\"10\"},\"end\":{\"hour\":\"11\",\"min\":\"10\"}},{\"start\":{\"hour\":\"10\",\"min\":\"00\"},\"end\":{\"hour\":\"11\",\"min\":\"00\"}}]");
        }

        [Test]
        public void TestStartGreaterThanEnd()
        {
            var notification = new AndroidNotification("title", "content");
            notification.AddAcceptTime(14, 0, 12, 0);
            var time = JsonConvert.SerializeObject(notification.CustomItems["accept_time"]);
            Assert.AreEqual(time,
                "[{\"start\":{\"hour\":\"14\",\"min\":\"00\"},\"end\":{\"hour\":\"23\",\"min\":\"59\"}}]");
        }

        [Test]
        public void TestNotificationIsEmpty()
        {
            AndroidNotification android = new AndroidNotification();

            Assert.IsTrue(android.IsEmpty);

            android.Title = "title";
            android.Content = "content";

            Assert.IsFalse(android.IsEmpty);

            PayloadNotification ios = new PayloadNotification(null);

            Assert.IsTrue(ios.IsEmpty);

            ios.Alert.Body = "abc";

            Assert.IsFalse(ios.IsEmpty);
        }
    }
}