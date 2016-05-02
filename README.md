# PushNotifications

PushNotifications SDK 封装了腾讯信鸽推送服务端所有 v2.0 API RestAPI (http://xg.qq.com/) .
基于.NET 4.5框架。

[![AppVeyor](https://img.shields.io/appveyor/ci/gruntjs/grunt.svg?maxAge=2592000?style=flat-square)](https://ci.appveyor.com/project/seamys/pushnotifications)
[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg?maxAge=2592000?style=flat-square)](https://www.nuget.org/packages/PushNotifications.Xinge/)

## 快速安装

使用 [Nuget](https://www.nuget.org/packages/PushNotifications.Xinge/) 安装

``` shell
Install-Package PushNotifications.Xinge
```
### 简单使用

####  1. 推送到所有Android设备
``` csharp
//推送到所有Android设备
var client = new PushClient(AndroidAccessId, AndroidSecretKey);
var androidMsg = new AndroidNotification
{
    Title = "测试消息",
    Content = "这是一条测试消息"
};
var result = client.PushAllDeviceAsync(androidMsg);
Console.WriteLine(result.Result);
```
####  2. 推送到所有IOS设备
``` csharp
var client = new PushClient(IosAccessId, IosSecretKey);
var payload = new PayloadNotification("这是一条测试消息", 1);
var result = client.PushAllDeviceAsync(payload);
Console.WriteLine(result.Result);
```
#### 3. 更多例子
[/src/PushNotifications.Examples](https://github.com/seamys/PushNotifications/tree/master/src/PushNotifications.Examples)

## 修改日志
[release notes](https://github.com/seamys/PushNotifications/releases).
## 联系我

**Email: yiim@foxmail.com**
**Web: http://www.frllk.com**
## License
[MIT](http://opensource.org/licenses/MIT)

Copyright (c) 2016 - seamys