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
using PushNotifications.Schema;

namespace PushNotifications.Examples
{
    internal class Program
    {
        protected static string AndroidAccessId = "<Your Android access Id>";

        protected static string AndroidSecretKey = "<Your Android secret key>";

        protected static string IosAccessId = "<Your IOS access Id>";

        protected static string IosSecretKey = "<Your IOS secret key>";

        private static void Main(string[] args)
        {
            //推送到所有Android设备
            SimpleToPushAllDevice();

            //推送指定设备
            SimpleToPushSingleDevice();

            //创建复杂的Android Message消息
            CreateComplexAndroidMessage();

            //创建复杂的IOS Message消息
            CreateComplexIosMessage();

            //注册http 请求事件
            CreateHttpEventHandler();

        }

        /// <summary>
        /// 推送到所有Android设备
        /// </summary>
        private static void SimpleToPushAllDevice()
        {
            //推送到所有Android设备
            var client = new PushClient(AndroidAccessId, AndroidSecretKey);
            var androidMsg = new AndroidNotification
            {
                Title = "测试消息",
                Content = "这是一条测试消息"
            };
            var result = client.PushAllDeviceAsync(androidMsg);
            Console.WriteLine(result.Result);

            // 推送到所有IOS设备
            client = new PushClient(IosAccessId, IosSecretKey);
            var payload = new PayloadNotification("这是一条测试消息", 1);
            result = client.PushAllDeviceAsync(payload);
            Console.WriteLine(result.Result);
        }

        /// <summary>
        /// 推送指定设备
        /// </summary>
        private static void SimpleToPushSingleDevice()
        {
            //推送到指定Android设备
            var AndroiddeviceToken = "xxxxxxxxxxxxx";
            var client = new PushClient(AndroidAccessId, AndroidSecretKey);
            var androidMsg = new AndroidNotification
            {
                Title = "测试消息",
                Content = "这是一条测试消息"
            };
            var result = client.PushSingleDeviceAsync(AndroiddeviceToken, androidMsg);
            Console.WriteLine(result.Result);

            // 推送到指定IOS设备
            var iosDeviceToken = "xxxxxxxxxxxxxxxxxx";
            client = new PushClient(IosAccessId, IosSecretKey);
            var payload = new PayloadNotification("这是一条测试消息", 1);
            result = client.PushSingleDeviceAsync(iosDeviceToken, payload);

            Console.WriteLine(result.Result);
        }

        /// <summary>
        /// 创建复杂的Android Message消息
        /// {
        /// "title": "标题",
        /// "content": "内容",
        /// "accept_time": [
        /// {
        /// "start": {
        /// "hour": "20",
        /// "min": "00"
        /// },
        /// "end": {
        /// "hour": "23",
        /// "min": "00"
        /// }
        /// },
        /// {
        /// "start": {
        /// "hour": "12",
        /// "min": "00"
        /// },
        /// "end": {
        /// "hour": "13",
        /// "min": "00"
        /// }
        /// }
        /// ],
        /// "n_id": 1,
        /// "builder_id": 0,
        /// "ring": 1,
        /// "ring_raw": "ring",
        /// "vibrate": 1,
        /// "lights": 1,
        /// "clearable": 1,
        /// "icon_type": 0,
        /// "icon_res": "xg",
        /// "style_id": 1,
        /// "small_icon": "xg",
        /// "action": {
        /// "action_type": 1,
        /// "activity": "xxxxx",
        /// "aty_attr": {
        /// "if": 0,
        /// "pf": 0
        /// },
        /// "browser": {
        /// "url": "https://github.com/seamys",
        /// "confirm": 1
        /// },
        /// "intent": "xxx"
        /// },
        /// "custom_content": {
        /// "custom_key1": "自定义值1",
        /// "custom_key2": "自定义值2"
        /// }
        /// }
        /// </summary>
        private static void CreateComplexAndroidMessage()
        {
            var notification = new AndroidNotification("标题", "内容");

            // 指定在晚上8点到23点间推送
            notification.AddAcceptTime(20, 23);

            // 或者中午的12点到13点间推送
            notification.AddAcceptTime(12, 13);

            // 通知id，选填。若大于0，则会覆盖先前弹出的相同id通知；若为0，展示本条通知且不影响其他通知；若为-1，
            // 将清除先前弹出的所有通知，仅展示本条通知。默认为0
            notification.AddCustom("n_id", 1);

            //本地通知样式，必填
            notification.AddCustom("builder_id", 0);

            // 是否响铃，0否，1是，下同.
            notification.AddCustom("ring", 1);

            // 指定应用内的声音（ring.mp3）.
            notification.AddCustom("ring_raw", "ring");

            // 是否振动，选填，默认1
            notification.AddCustom("vibrate", 1);

            // 设置呼吸灯
            notification.AddCustom("lights", 1);

            // 通知栏是否可以清楚
            notification.AddCustom("clearable", 1);

            // 通知栏图标是应用内图标还是上传图标,0是应用内图标，1是上传图标,选填
            notification.AddCustom("icon_type", 0);

            // 应用内图标文件名（xg.png）或者下载图标的url地址，选填
            notification.AddCustom("icon_res", "xg");

            // Web端设置是否覆盖编号的通知样式，默认1，0否，1是,选填
            notification.AddCustom("style_id", 1);

            // 指定状态栏的小图片(xg.png),选填
            notification.AddCustom("small_icon", "xg");

            // 动作，选填。默认为打开app
            notification.AddCustom("action", new
            {
                action_type = 1,
                activity = "xxxxx",
                aty_attr = new
                {
                    @if = 0,
                    pf = 0
                },
                browser = new
                {
                    url = "https://github.com/seamys",
                    confirm = 1
                },
                intent = "xxx"
            });


            /*** 不在腾讯设置的key 都会放到 custom_content 字段内 ***/

            //自定义值1
            notification.AddCustom("custom_key1", "自定义值1");

            //自定义值1
            notification.AddCustom("custom_key2", "自定义值2");

            var json = notification.ToJson();
            //打印消息格式
            Console.WriteLine(json);
        }

        /// <summary>
        /// 复杂 ios APNS payload
        /// {
        /// "aps": {
        /// "alert": {
        /// "loc-key": "GAME_PLAY_REQUEST_FORMAT",
        /// "loc-args": [
        /// "Jenna",
        /// "Frank"
        /// ],
        /// "body": "测试消息",
        /// "action-loc-key": "PLAY"
        /// },
        /// "badge": 1,
        /// "sound": "chime"
        /// },
        /// "custom_key1": [
        /// "123",
        /// "123"
        /// ]
        /// }
        /// </summary>
        private static void CreateComplexIosMessage()
        {
            var notification = new PayloadNotification("测试消息", 1);

            // action-loc-key
            notification.Alert.ActionLocalizedKey = "PLAY";

            // loc-key
            notification.Alert.LocalizedKey = "GAME_PLAY_REQUEST_FORMAT";

            // loc-args
            notification.Alert.AddLocalizedArgs("Jenna", "Frank");

            // sound
            notification.Sound = "chime";

            //自定义值
            notification.AddCustom("custom_key1", "自定义值");
            notification.AddCustom("custom_key1", new[] { "123", "123" });

            var json = notification.ToJson();

            Console.WriteLine(json);
        }

        private static void CreateHttpEventHandler()
        {
            // 推送到所有IOS设备
            var client = new PushClient(IosAccessId, IosSecretKey);

            client.HttpCallback += Client_HttpCallback;

            var payload = new PayloadNotification("这是一条测试消息", 1);
            var result = client.PushAllDeviceAsync(payload);
            Console.WriteLine(result.Result);
        }

        private static void Client_HttpCallback(string url, Dictionary<string, string> param, string content)
        {
            Console.WriteLine(url);
            Console.WriteLine(content);
        }
    }
}