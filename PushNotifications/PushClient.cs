using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushNotifications.Schema;
using GV = PushNotifications.GlobalVariables;

namespace PushNotifications
{
    public class PushClient
    {
        protected string SecretKey;
        protected string AccessId;
        protected uint ExpireTime;
        public bool IsDevelopment;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="secretKey"></param>
        /// <param name="expireTime"></param>
        public PushClient(string accessId, string secretKey, uint expireTime = 24 * 60 * 60)
        {
            SecretKey = secretKey;
            AccessId = accessId;
            ExpireTime = expireTime;
        }
        /// <summary>
        /// 推送单一设备消息
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="msg"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public Task<PushResult> PushSingleDeviceAsync(string deviceToken, Notification msg, MessageType messageType = MessageType.Notification)
        {
            string message = msg.ToJson();
            Dictionary<string, string> param = InitParams();
            param.Add("message", message);
            param.Add("device_token", deviceToken);
            param.Add("message_type", messageType.ToString());
            param.Add("expire_time", ExpireTime.ToString());
            param.Add("send_time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (msg is AndroidNotification)
                param.Add("multi_pkg", "1");
            param.Add("environment", IsDevelopment ? "1" : "2");
            return RestfulPost<PushResult>(GV.RESTAPI_PUSHSINGLEDEVICE, param);
        }
        /// <summary>
        /// 单个帐号推送
        /// </summary>
        /// <remarks>设备的账户或别名由终端SDK在调用推送注册接口时设置，详情参考终端SDK文档。</remarks>
        /// <param name="account">针对某一账号推送，帐号可以是qq号，邮箱号，openid，手机号等各种类型</param>
        /// <param name="msg">消息体</param>
        /// <param name="messageType">消息类型：1：通知 2：透传消息</param>
        /// <returns></returns>
        public Task<PushResult> PushSingleAccountAsync(string account, Notification msg, MessageType messageType = MessageType.Notification)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 批量帐号
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="msg"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public Task<PushResult> PushMultiAccountAsync(List<string> accounts, Notification msg, MessageType messageType = MessageType.Notification)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public Task<PushResult> PushAllDevice(Notification msg, MessageType messageType = MessageType.Notification)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushId"></param>
        /// <returns></returns>
        public Task<PushResult> GetNotificationsStatus(string pushId)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 标签
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="operators"></param>
        /// <param name="msg"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public Task<PushResult> PushTagsDevice(List<string> tags, Operators operators, Notification msg, MessageType messageType = MessageType.Notification)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 推送到多个设备
        /// </summary>
        /// <param name="devices">token集合</param>
        /// <param name="msg">消息</param>
        /// <param name="messageType">通知还是透传消息</param>
        /// <returns></returns>
        public async Task<PushResult> PushMultiDeviceAsync(List<string> devices, Notification msg, MessageType messageType = MessageType.Notification)
        {
            string pushId = await CreateMultiPushAsync(messageType, msg);
            if (string.IsNullOrWhiteSpace(pushId))
                return new PushResult();
            Dictionary<string, string> param = InitParams();
            param.Add("message", msg.ToJson());
            param.Add("device_list", JsonConvert.SerializeObject(devices));
            param.Add("push_id", pushId);
            return await RestfulPost<PushResult>(GV.RESTAPI_PUSHDEVICELISTMULTIPLE, param);
        }
        /// <summary>
        /// 创建多条推送Id
        /// </summary>
        /// <returns></returns>
        public async Task<string> CreateMultiPushAsync(MessageType type, Notification msg)
        {
            Dictionary<string, string> param = InitParams();
            param.Add("expire_time", ExpireTime.ToString());
            if (msg is AndroidNotification)
                param.Add("multi_pkg", "1");
            param.Add("message_type", type.ToString());
            param.Add("message", msg.ToJson());
            string content = await RestfulPost(GV.RESTAPI_CREATEMULTIPUSH, param);
            JObject obj = JObject.Parse(content);
            return obj["result"]["push_id"].Value<string>();
        }
        protected async Task<T> RestfulPost<T>(string url, Dictionary<string, string> param)
        {
            string content = await RestfulPost(url, param);
            return JsonConvert.DeserializeObject<T>(content);
        }
        protected async Task<string> RestfulPost(string url, Dictionary<string, string> param)
        {
            PushResult result = new PushResult();
            string sign = Signature(url, "POST", param);
            param.Add("sign", sign);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync(url, new FormUrlEncodedContent(param));
                return await response.Content.ReadAsStringAsync();
            }
        }
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected string Signature(string url, string method, Dictionary<string, string> param)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(method);
            builder.Append(url.Replace("http://", ""));
            foreach (var item in param.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                builder.Append($"{item.Key}={item.Value}");
            }
            builder.Append(SecretKey);
            return Md5(builder.ToString());
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> InitParams()
        {
            string currentTimestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds.ToString(CultureInfo.InvariantCulture);
            return new Dictionary<string, string>()
            {
                { "access_id", AccessId },
                { "timestamp", currentTimestamp }
            };
        }
        protected string Md5(string source)
        {
            var provider = new MD5CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(source);
            var builder = new StringBuilder();
            bytes = provider.ComputeHash(bytes);
            foreach (var b in bytes)
                builder.Append(b.ToString("x2").ToLower());
            return builder.ToString();
        }
    }
}
