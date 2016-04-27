using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PushNotifications.Models;
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
        /// <returns></returns>
        public PushResult PushSingleDevice(string deviceToken, Notification msg)
        {
            string message = msg.ToJson();
            Dictionary<string, string> param = InitParams();
            param.Add("message", message);
            param.Add("device_token", deviceToken);
            param.Add("message_type", "0");
            param.Add("expire_time", ExpireTime.ToString());
            param.Add("send_time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (msg is AndroidNotification)
                param.Add("multi_pkg", "1");
            param.Add("environment", IsDevelopment ? "1" : "2");
            return RestfulPost(GV.RESTAPI_PUSHSINGLEDEVICE, param).Result;
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
        protected async Task<PushResult> RestfulPost(string url, Dictionary<string, string> param)
        {
            PushResult result = new PushResult();
            string sign = Signature(url, "POST", param);
            param.Add("sign", sign);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync(url, new FormUrlEncodedContent(param));
                if (response.Content == null) return result;
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PushResult>(content);
            }
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> InitParams()
        {
            string currentTimestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds.ToString(CultureInfo.InvariantCulture);
            return new Dictionary<string, string>() { { "access_id", AccessId }, { "timestamp", currentTimestamp } };
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
