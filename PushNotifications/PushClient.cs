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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="content"></param>
    public delegate void HttpEvent(string url, Dictionary<string, string> param, string content);
    public class PushClient
    {
        protected string SecretKey;
        protected string AccessId;
        protected uint ExpireTime;
        public bool IsDevelopment;
        public event HttpEvent HttpCallback;
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
        public Task<string> PushSingleDeviceAsync(string deviceToken, Notification msg)
        {
            var param = InitParams(msg);
            param.Add("device_token", deviceToken);
            return RestfulPost(GV.RESTAPI_PUSHSINGLEDEVICE, param);
        }
        /// <summary>
        /// 单个帐号推送
        /// </summary>
        /// <remarks>设备的账户或别名由终端SDK在调用推送注册接口时设置，详情参考终端SDK文档。</remarks>
        /// <param name="account">针对某一账号推送，帐号可以是qq号，邮箱号，openid，手机号等各种类型</param>
        /// <param name="msg">消息体</param>
        /// <returns></returns>
        public Task<string> PushSingleAccountAsync(string account, Notification msg)
        {
            var param = InitParams(msg);
            param.Add("account", account);
            return RestfulPost(GV.RESTAPI_PUSHSINGLEACCOUNT, param);
        }
        /// <summary>
        /// 批量帐号
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<string> PushMultiAccountAsync(List<string> accounts, Notification msg)
        {
            string pushId = await CreateMultiPushAsync(msg);
            if (string.IsNullOrWhiteSpace(pushId))
                return JsonConvert.SerializeObject(new { ret_code = -1 });
            var param = InitParams(msg);
            param.Add("account_list", JsonConvert.SerializeObject(accounts));
            param.Add("push_id", pushId);
            return await RestfulPost(GV.RESTAPI_PUSHACCOUNTLISTMULTIPLE, param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task<string> PushAllDeviceAsync(Notification msg)
        {
            var param = InitParams(msg);
            return RestfulPost(GV.RESTAPI_PUSHALLDEVICE, param);
        }
        /// <summary>
        /// 标签
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="msg"></param>
        /// <param name="operators"></param>
        /// <param name="loopTimes"></param>
        /// <param name="loopInterval"></param>
        /// <returns></returns>
        public Task<string> PushTagsDeviceAsync(List<string> tags, Notification msg, Operators operators = Operators.OR, int loopTimes = 0, int loopInterval = 0)
        {
            var param = InitParams(msg);
            param.Add("tags_list", JsonConvert.SerializeObject(tags));
            param.Add("tags_op", operators == Operators.AND ? "AND" : "OR");
            if (loopTimes > 0 && loopInterval < 16 && loopInterval > 0 && loopInterval < 15)
            {
                param.Add("loop_times", loopTimes.ToString());
                param.Add("loop_interval", loopTimes.ToString());
            }
            return RestfulPost(GV.RESTAPI_PUSHALLDEVICE, param);
        }
        /// <summary>
        /// 推送到多个设备
        /// </summary>
        /// <param name="devices">token集合</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public async Task<string> PushMultiDeviceAsync(List<string> devices, Notification msg)
        {
            string pushId = await CreateMultiPushAsync(msg);
            var param = InitParams();
            param.Add("device_list", JsonConvert.SerializeObject(devices));
            param.Add("push_id", pushId);
            return await RestfulPost(GV.RESTAPI_PUSHDEVICELISTMULTIPLE, param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushId"></param>
        /// <returns></returns>
        public Task<string> QueryPushStatusAsync(List<string> pushId)
        {
            JArray array = new JArray();
            pushId.ForEach(x => array.Add(new JObject(new { push_id = x })));
            var param = InitParams();
            param.Add("push_id", JsonConvert.SerializeObject(pushId));
            return RestfulPost(GV.RESTAPI_QUERYPUSHSTATUS, param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string> QueryDeviceCountAsync()
        {
            var param = InitParams();
            return RestfulPost(GV.RESTAPI_QUERYDEVICECOUNT, param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Task<string> QueryTagsTokenAsync(uint start, uint limit)
        {
            var param = InitParams();
            param.Add("start", start > 0 ? start : 0);
            param.Add("start", limit > 100 ? 100 : limit);
            return RestfulPost(GV.RESTAPI_QUERYTAGTOKENNUM, param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<string> QueryTokenTagsAsync(string deviceToken)
        {
            var param = InitParams();
            param.Add("device_token", deviceToken);
            return RestfulPost(GV.RESTAPI_QUERYTAGTOKENNUM, param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushId"></param>
        /// <returns></returns>
        public Task<string> CancelTimingTaskAsync(string pushId)
        {
            var param = InitParams();
            param.Add("push_id", pushId);
            return RestfulPost(GV.RESTAPI_CANCELTIMINGPUSH, param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public Task<string> SetTagsAsync(Dictionary<string, List<string>> tags)
        {
            var param = InitParams();
            string tagsParam = ToTagParams(tags);
            param.Add("tag_token_list", tagsParam);
            return RestfulPost(GV.RESTAPI_BATCHSETTAG, param);
        }
        private string ToTagParams(Dictionary<string, List<string>> tags)
        {
            List<List<string>> list = new List<List<string>>();

            foreach (var item in tags)
            {
                if (string.IsNullOrWhiteSpace(item.Key) || item.Value == null || item.Value.Count == 0)
                    continue;
                List<string> sub = new List<string> { item.Key };
                item.Value.ForEach(x => sub.Add(x));
                list.Add(sub);
            }
            return JsonConvert.SerializeObject(list);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public Task<string> DeleteTagsAsync(Dictionary<string, List<string>> tags)
        {
            var param = InitParams();
            string tagsParam = ToTagParams(tags);
            param.Add("tag_token_list", tagsParam);
            return RestfulPost(GV.RESTAPI_BATCHDELTAG, param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        public Task<string> QueryTokenAsync(string deviceToken)
        {
            var param = InitParams();
            param.Add("device_token", deviceToken);
            return RestfulPost(GV.RESTAPI_QUERYINFOOFTOKEN, param);
        }
        public Task<string> DeleteOfflineAsync(string pushId)
        {
            var param = InitParams();
            param.Add("push_id", pushId);
            return RestfulPost(GV.RESTAPI_DELETEOFFLINEPUSH, param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        public Task<string> DeleteAccountTokensAsync(string account, string deviceToken)
        {
            var param = InitParams();
            param.Add("account", account);
            param.Add("device_token", deviceToken);
            return RestfulPost(GV.RESTAPI_DELETETOKENOFACCOUNT, param);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        public Task<string> DeleteAccountTokensAsync(string account)
        {
            var param = InitParams();
            param.Add("account", account);
            return RestfulPost(GV.RESTAPI_DELETEALLTOKENSOFACCOUNT, param);
        }
        /// <summary>
        /// 创建多条推送Id
        /// </summary>
        /// <returns></returns>
        public Task<string> CreateMultiPushAsync(Notification msg)
        {
            var param = InitParams();
            param.Remove("message");
            return RestfulPost(GV.RESTAPI_CREATEMULTIPUSH, param);
        }
        protected async Task<string> RestfulPost(string url, Dictionary<string, object> param)
        {
            string sign = Signature(url, "POST", param);
            param.Add("sign", sign);
            using (HttpClient client = new HttpClient())
            {
                var httpParam = param.Select(x => x).ToDictionary(x => x.Key, v => v.Value.ToString());
                var response = await client.PostAsync(url, new FormUrlEncodedContent(httpParam));
                string content = await response.Content.ReadAsStringAsync();
                HttpCallback?.Invoke(url, httpParam, content);
                return content;
            }
        }
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected string Signature(string url, string method, Dictionary<string, object> param)
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
        protected Dictionary<string, object> InitParams(Notification msg = null)
        {
            string currentTimestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds.ToString(CultureInfo.InvariantCulture);
            var param = new Dictionary<string, object>()
            {
                { "access_id", AccessId },
                { "timestamp", currentTimestamp }
            };
            if (msg != null)
            {
                param.Add("message", msg);
                param.Add("message_type", msg.MessageType);
                param.Add("expire_time", ExpireTime);
                if (msg is AndroidNotification)
                    param.Add("multi_pkg", 1);
                else
                    param.Add("environment", IsDevelopment ? 1 : 2);
            }
            return param;
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
        public int GetState(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return -1;
            JToken token = JToken.Parse(content);
            return token["ret_code"].Value<int>();
        }

        public string GetPushId(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return null;
            JToken token = JToken.Parse(content);
            return token["result"]["push_id"].Value<string>();
        }
    }
}
