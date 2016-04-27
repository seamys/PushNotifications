# PushNotifications

腾讯信鸽推送


``` csharp

   PushClient client = new PushClient("<accessId>", "<secretKey>");
   //全量推送
   client.PushAllDeviceAsync(new PayloadNotification("test message", 1)).Wait();
   //推送单条消息
   client.PushSingleDeviceAsync("<deviceToken>", new PayloadNotification("test message", 1));

```