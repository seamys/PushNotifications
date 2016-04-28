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
    /// 腾讯信鸽推送服务端SDK
    /// </summary>
    public class PushClient
    {
        /// <summary>
        ///  SecretKey
        /// </summary>
        protected string SecretKey;
        /// <summary>
        ///  AccessId
        /// </summary>
        protected string AccessId;
        /// <summary>
        /// ExpireTime 通知在腾讯服务器中存储的时间
        /// </summary>
        protected uint ExpireTime;
        /// <summary>
        /// 是否是开发环境
        /// </summary>
        public bool IsDevelopment;
        /// <summary>
        /// Http 响应完成后会触发此事件
        /// </summary>
        public event HttpEvent HttpCallback;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="accessId">accessId</param>
        /// <param name="secretKey">secretKey</param>
        /// <param name="expireTime">通知在腾讯服务器中存储的时间</param>
        public PushClient(string accessId, string secretKey, uint expireTime = 24 * 60 * 60)
        {
            SecretKey = secretKey;
            AccessId = accessId;
            ExpireTime = expireTime;

        }
        /// <summary>
        /// 推送单一设备消息
        /// URL: /v2/push/single_device
        /// </summary>
        /// <param name="deviceToken">设备标示 device_token</param>
        /// <param name="msg">推送通知</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> PushSingleDeviceAsync(string deviceToken, Notification msg)
        {
            var param = InitParams(msg);
            param.Add("device_token", deviceToken);
            return RestfulPost(GV.RESTAPI_PUSHSINGLEDEVICE, param);
        }
        /// <summary>
        /// 单个帐号推送
        /// URL: /v2/push/single_account
        /// </summary>
        /// <remarks>设备的账户或别名由终端SDK在调用推送注册接口时设置，详情参考终端SDK文档。</remarks>
        /// <param name="account">针对某一账号推送，帐号可以是qq号，邮箱号，openid，手机号等各种类型</param>
        /// <param name="msg">消息体</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> PushSingleAccountAsync(string account, Notification msg)
        {
            var param = InitParams(msg);
            param.Add("account", account);
            return RestfulPost(GV.RESTAPI_PUSHSINGLEACCOUNT, param);
        }
        /// <summary>
        /// 批量帐号
        /// URL: /v2/push/account_list_multiple
        /// </summary>
        /// <param name="accounts">用户设备列表别名</param>
        /// <param name="msg">推送通知</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public async Task<string> PushMultiAccountAsync(List<string> accounts, Notification msg)
        {
            string pushId = GetPushId(await CreateMultiPushAsync(msg));
            if (string.IsNullOrWhiteSpace(pushId))
                return JsonConvert.SerializeObject(new { ret_code = -1 });
            var param = InitParams(msg);
            param.Add("account_list", JsonConvert.SerializeObject(accounts));
            param.Add("push_id", pushId);
            return await RestfulPost(GV.RESTAPI_PUSHACCOUNTLISTMULTIPLE, param);
        }
        /// <summary>
        /// 全量设备推送
        /// URL: /v2/push/all_device
        /// </summary>
        /// <param name="msg">推送通知</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> PushAllDeviceAsync(Notification msg)
        {
            var param = InitParams(msg);
            return RestfulPost(GV.RESTAPI_PUSHALLDEVICE, param);
        }
        /// <summary>
        /// 指定标签推送
        /// URL: /v2/push/tags_device
        /// </summary>
        /// <param name="tags">指定标签列表</param>
        /// <param name="msg">消息体</param>
        /// <param name="operators">AND或OR</param>
        /// <param name="loopTimes">循环任务执行的次数，取值[1, 15]</param>
        /// <param name="loopInterval">循环任务的执行间隔，以天为单位，取值[1, 14]。loop_times和loop_interval一起表示任务的生命周期，不可超过14天</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
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
            return RestfulPost(GV.RESTAPI_PUSHTAGS, param);
        }
        /// <summary>
        /// 推送到多个设备
        /// URL: /v2/push/create_multipush
        ///      /v2/push/device_list_multiple
        /// </summary>
        /// <param name="devices">token集合,单次发送token不超过1000个</param>
        /// <param name="msg">消息体</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public async Task<string> PushMultiDeviceAsync(List<string> devices, Notification msg)
        {
            string pushId = GetPushId(await CreateMultiPushAsync(msg));
            var param = InitParams();
            param.Add("device_list", JsonConvert.SerializeObject(devices));
            param.Add("push_id", pushId);
            return await RestfulPost(GV.RESTAPI_PUSHDEVICELISTMULTIPLE, param);
        }
        /// <summary>
        /// 查询群发消息发送状态
        /// URL: /v2/push/get_msg_status
        /// </summary>
        /// <param name="pushIds">推送任务id集合</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> QueryPushStatusAsync(List<string> pushIds)
        {
            JArray array = new JArray();
            pushIds.ForEach(x => array.Add(new JObject(new { push_id = x })));
            var param = InitParams();
            param.Add("push_id", JsonConvert.SerializeObject(array));
            return RestfulPost(GV.RESTAPI_QUERYPUSHSTATUS, param);
        }
        /// <summary>
        /// 查询应用覆盖的设备数
        /// URL: /v2/application/get_app_device_num
        /// </summary>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> QueryDeviceCountAsync()
        {
            var param = InitParams();
            return RestfulPost(GV.RESTAPI_QUERYDEVICECOUNT, param);
        }
        /// <summary>
        /// 查询应用某个标签下关联的设备数
        /// URL: /v2/tags/query_tag_token_num
        /// </summary>
        /// <param name="start">开始值,默认0 </param>
        /// <param name="limit">限制数量</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> QueryTagsTokenAsync(uint start, uint limit)
        {
            var param = InitParams();
            param.Add("start", start > 0 ? start : 0);
            param.Add("start", limit > 100 ? 10 : limit);
            return RestfulPost(GV.RESTAPI_QUERYTAGTOKENNUM, param);
        }
        /// <summary>
        /// 查询应用的某个设备上设置的标签
        /// URL: /v2/tags/query_token_tags
        /// </summary>
        /// <param name="deviceToken">device_token</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> QueryTokenTagsAsync(string deviceToken)
        {
            var param = InitParams();
            param.Add("device_token", deviceToken);
            return RestfulPost(GV.RESTAPI_QUERYTOKENTAGS, param);
        }
        /// <summary>
        /// 取消尚未触发的定时群发任务
        /// URL: /v2/push/cancel_timing_task
        /// </summary>
        /// <param name="pushId">任务Id</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> CancelTimingTaskAsync(string pushId)
        {
            var param = InitParams();
            param.Add("push_id", pushId);
            return RestfulPost(GV.RESTAPI_CANCELTIMINGPUSH, param);
        }
        /// <summary>
        /// 批量设置标签
        /// URL: /v2/tags/batch_set
        /// </summary>
        /// <param name="tags">每次调用最多允许设置20对，每个对里面标签在前，token在后。注意标签最长50字节，不可包含空格；真实token长度至少40字节</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> SetTagsAsync(Dictionary<string, List<string>> tags)
        {
            var param = InitParams();
            string tagsParam = ToTagParams(tags);
            param.Add("tag_token_list", tagsParam);
            return RestfulPost(GV.RESTAPI_BATCHSETTAG, param);
        }
        /// <summary>
        /// 批量删除标签(/v2/tags/batch_del)
        /// </summary>
        /// <param name="tags">每次调用最多允许设置20对，每个对里面标签在前，token在后。注意标签最长50字节，不可包含空格；真实token长度至少40字节。</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> DeleteTagsAsync(Dictionary<string, List<string>> tags)
        {
            var param = InitParams();
            string tagsParam = ToTagParams(tags);
            param.Add("tag_token_list", tagsParam);
            return RestfulPost(GV.RESTAPI_BATCHDELTAG, param);
        }
        /// <summary>
        /// 查询应用的某个token的信息(/v2/application/get_app_token_info)
        /// </summary>
        /// <param name="deviceToken">device_token</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> QueryTokenAsync(string deviceToken)
        {
            var param = InitParams();
            param.Add("device_token", deviceToken);
            return RestfulPost(GV.RESTAPI_QUERYINFOOFTOKEN, param);
        }
        /// <summary>
        /// 删除群发推送任务的离线消息(/v2/push/delete_offline_msg)
        /// </summary>
        /// <param name="pushId">任务ID</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> DeleteOfflineAsync(string pushId)
        {
            var param = InitParams();
            param.Add("push_id", pushId);
            return RestfulPost(GV.RESTAPI_DELETEOFFLINEPUSH, param);
        }
        /// <summary>
        /// 删除应用中某个account映射的某个token(/v2/application/del_app_account_tokens)
        /// </summary>
        /// <param name="account">设备别名</param>
        /// <param name="deviceToken">device_token</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> DeleteAccountTokensAsync(string account, string deviceToken)
        {
            var param = InitParams();
            param.Add("account", account);
            param.Add("device_token", deviceToken);
            return RestfulPost(GV.RESTAPI_DELETETOKENOFACCOUNT, param);
        }
        /// <summary>
        /// 删除应用中某account映射的所有token
        /// URL: /v2/application/del_app_account_all_tokens
        /// </summary>
        /// <param name="account">设备别名(账号)</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> DeleteAccountTokensAsync(string account)
        {
            var param = InitParams();
            param.Add("account", account);
            return RestfulPost(GV.RESTAPI_DELETEALLTOKENSOFACCOUNT, param);
        }
        /// <summary>
        /// 创建多条推送Id
        /// URL: /v2/push/create_multipush
        /// </summary>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> CreateMultiPushAsync(Notification msg)
        {
            var param = InitParams(msg);
            return RestfulPost(GV.RESTAPI_CREATEMULTIPUSH, param);
        }
        /// <summary>
        /// Http 推送方法
        /// </summary>
        /// <param name="url">访问URL</param>
        /// <param name="param">http 参数</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
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
        /// 格式化标签
        /// </summary>
        /// <param name="tags">标签设备分组</param>
        /// <returns>格式化后的json 字符串</returns>
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
        ///  获取签名
        /// </summary>
        /// <param name="url">访问URL</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="param">http 参数</param>
        /// <returns>返回签名</returns>
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
        /// <returns>通用参数</returns>
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

                param.Add("expire_time", ExpireTime);
                if (msg is AndroidNotification)
                {
                    param.Add("message_type", msg.MessageType == MessageType.Notification ? 1 : 2);
                    param.Add("multi_pkg", 1);
                }
                else
                {
                    param.Add("message_type", 0);
                    param.Add("environment", IsDevelopment ? 1 : 2);
                }
            }
            return param;
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected string GetPushId(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return null;
            JToken token = JToken.Parse(content);
            return token["result"]["push_id"].Value<string>();
        }
    }
    /// <summary>
    /// http时间委托
    /// </summary>
    /// <param name="url">访问地址</param>
    /// <param name="param">http 提交参数</param>
    /// <param name="content">响应内容</param>
    public delegate void HttpEvent(string url, Dictionary<string, string> param, string content);
}
