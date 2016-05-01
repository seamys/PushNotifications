# PushNotifications

腾讯信鸽推送 

[![Build status](https://ci.appveyor.com/api/projects/status/u8nrbj9yr5lt34vw?svg=true)](https://ci.appveyor.com/project/seamys/pushnotifications)

``` csharp

   PushClient client = new PushClient("<accessId>", "<secretKey>");
   //全量推送
   client.PushAllDeviceAsync(new PayloadNotification("test message", 1)).Wait();
   //推送单条消息
   client.PushSingleDeviceAsync("<deviceToken>", new PayloadNotification("test message", 1));

```
