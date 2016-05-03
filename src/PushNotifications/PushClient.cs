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
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushNotifications.Schema;

namespace PushNotifications
{
    using GV = GlobalVariables;

    /// <summary>
    ///     http时间委托
    /// </summary>
    /// <param name="url">访问地址</param>
    /// <param name="param">http 提交参数</param>
    /// <param name="content">响应内容</param>
    public delegate void HttpEvent(string url, Dictionary<string, string> param, string content);

    /// <summary>
    ///     腾讯信鸽推送服务端SDK
    /// </summary>
    public class PushClient
    {
        /// <summary>
        /// AccessId
        /// </summary>
        protected string AccessId;

        /// <summary>
        /// 通知在腾讯服务器中存储的时间
        /// </summary>
        protected uint ExpireTime;

        /// <summary>
        /// 是否是开发环境
        /// </summary>
        protected bool IsDevelopment;

        /// <summary>
        /// SecretKey
        /// </summary>
        protected string SecretKey;

        /// <see cref="PushClient" />
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="accessId">accessId</param>
        /// <param name="secretKey">secretKey</param>
        /// <param name="expireTime">通知在腾讯服务器中存储的时间</param>
        /// <see cref="PushClient" />
        public PushClient(string accessId, string secretKey, uint expireTime = 86400)
        {
            SecretKey = secretKey;
            AccessId = accessId;
            ExpireTime = expireTime;
        }

        /// <summary>
        /// Http 响应完成后会触发此事件
        /// </summary>
        public event HttpEvent HttpCallback;

        /// <summary>
        /// http 处理类
        /// </summary>
        public HttpMessageHandler HttpHandler { get; set; }

        /// <summary>
        /// 时间差
        /// </summary>
        public uint Timestamp { get; set; }

        /// <summary>
        /// 配合timestamp确定请求的有效期，单位为秒，最大值为600。
        /// 若不设置此参数或参数值非法，则按默认值600秒计算有效期
        /// </summary>
        public uint ValidTime { get; set; }

        /// <summary>
        /// 设置是否是开发环境
        /// </summary>
        /// <param name="development">环境设置 true ? 2:1</param>
        /// <returns>当前对象</returns>
        public PushClient SetDevelopment(bool development)
        {
            IsDevelopment = development;
            return this;
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
            return RestfulPost(GV.PUSHSINGLEDEVICE, param);
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
            return PushSingleAccountAsync(account, msg, DateTime.Now.AddDays(-1));
        }

        /// <summary>
        /// 单个帐号指定时间推送，如果时间小于当前时间将立即发送
        /// URL: /v2/push/single_account
        /// </summary>
        /// <remarks>设备的账户或别名由终端SDK在调用推送注册接口时设置，详情参考终端SDK文档。</remarks>
        /// <param name="account">针对某一账号推送，帐号可以是qq号，邮箱号，openid，手机号等各种类型</param>
        /// <param name="msg">消息体</param>
        /// <param name="dateTime">指定发送时间</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> PushSingleAccountAsync(string account, Notification msg, DateTime dateTime)
        {
            var param = InitParams(msg);
            param.Add("account", account);
            if (dateTime > DateTime.Now)
            {
                param.Add("send_time", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            return RestfulPost(GV.PUSHSINGLEACCOUNT, param);
        }

        /// <summary>
        /// 批量帐号
        /// URL: /v2/push/account_list_multiple
        /// </summary>
        /// <param name="accounts">用户设备列表别名</param>
        /// <param name="msg">推送通知</param>
        /// <exception cref="Exception">推送失败</exception>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public async Task<string> PushMultiAccountAsync(List<string> accounts, Notification msg)
        {
            var pushId = Utils.GetPushId(await CreateMultiPushAsync(msg));
            if (string.IsNullOrWhiteSpace(pushId))
            {
                return JsonConvert.SerializeObject(new { ret_code = -1 });
            }
            var param = InitParams(msg);
            param.Add("account_list", JsonConvert.SerializeObject(accounts));
            param.Add("push_id", pushId);
            return await RestfulPost(GV.PUSHACCOUNTLISTMULTIPLE, param);
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
            return RestfulPost(GV.PUSHALLDEVICE, param);
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
                param.Add("loop_interval", loopInterval.ToString());
            }
            return RestfulPost(GV.PUSHTAGS, param);
        }

        /// <summary>
        /// 推送到多个设备
        /// URL: /v2/push/create_multipush
        /// /v2/push/device_list_multiple
        /// </summary>
        /// <param name="devices">token集合,单次发送token不超过1000个</param>
        /// <param name="msg">消息体</param>
        /// <exception cref="Exception">批量推送失败</exception>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public async Task<string> PushMultiDeviceAsync(List<string> devices, Notification msg)
        {
            var pushId = Utils.GetPushId(await CreateMultiPushAsync(msg));
            var param = InitParams();
            param.Add("device_list", JsonConvert.SerializeObject(devices));
            param.Add("push_id", pushId);
            return await RestfulPost(GV.PUSHDEVICELISTMULTIPLE, param);
        }

        /// <summary>
        /// 查询群发消息发送状态
        /// URL: /v2/push/get_msg_status
        /// </summary>
        /// <param name="pushIds">推送任务id集合</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> QueryPushStatusAsync(List<string> pushIds)
        {
            var array = new JArray();
            pushIds.ForEach(x => array.Add(new JObject(new { push_id = x })));
            var param = InitParams();
            param.Add("push_id", JsonConvert.SerializeObject(array));
            return RestfulPost(GV.QUERYPUSHSTATUS, param);
        }

        /// <summary>
        /// 查询应用覆盖的设备数
        /// URL: /v2/application/get_app_device_num
        /// </summary>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> QueryDeviceCountAsync()
        {
            var param = InitParams();
            return RestfulPost(GV.QUERYDEVICECOUNT, param);
        }

        /// <summary>
        /// 查询应用某个标签下关联的设备数
        /// URL: /v2/tags/query_tag_token_num
        /// </summary>
        /// <param name="tag">标签</param>
        /// <exception cref="ArgumentException">tag 不能为空或者 null </exception>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> QueryTagsTokenAsync(string tag)
        {
            TryThrowArgumentException(nameof(tag), tag);

            var param = InitParams();
            param.Add("tag", tag);
            return RestfulPost(GV.QUERYTAGTOKENNUM, param);
        }

        /// <summary>
        /// 查询应用的某个设备上设置的标签
        /// URL: /v2/tags/query_token_tags
        /// </summary>
        /// <param name="deviceToken">device_token</param>
        /// <exception cref="ArgumentException">deviceToken 为空或者 null</exception>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> QueryTokenTagsAsync(string deviceToken)
        {
            TryThrowArgumentException(nameof(deviceToken), deviceToken);

            var param = InitParams();
            param.Add("device_token", deviceToken);
            return RestfulPost(GV.QUERYTOKENTAGS, param);
        }

        /// <summary>
        /// 查询应用设置的标签
        /// URL: /v2/tags/query_app_tags
        /// </summary>
        /// <param name="start">开始值,默认0 </param>
        /// <param name="limit">限制数量</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> QueryTagsAsync(uint start, uint limit)
        {
            var param = InitParams();
            param.Add("start", start.ToString());
            param.Add("limit", (limit > 100 ? 100 : limit).ToString());
            return RestfulPost(GV.QUERYTAGS, param);
        }

        /// <summary>
        /// 取消尚未触发的定时群发任务
        /// URL: /v2/push/cancel_timing_task
        /// </summary>
        /// <param name="pushId">任务Id</param>
        /// <exception cref="ArgumentException">push_id 为空或者 null</exception>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> CancelTimingTaskAsync(string pushId)
        {
            TryThrowArgumentException(nameof(pushId), pushId);

            var param = InitParams();
            param.Add("push_id", pushId);
            return RestfulPost(GV.CANCELTIMINGPUSH, param);
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
            var tagsParam = Utils.ToTagParams(tags);
            param.Add("tag_token_list", tagsParam);
            return RestfulPost(GV.BATCHSETTAG, param);
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
            return RestfulPost(GV.QUERYINFOOFTOKEN, param);
        }

        /// <summary>
        /// 批量删除标签(/v2/tags/batch_del)
        /// </summary>
        /// <param name="tags">每次调用最多允许设置20对，每个对里面标签在前，token在后。注意标签最长50字节，不可包含空格；真实token长度至少40字节。</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> DeleteTagsAsync(Dictionary<string, List<string>> tags)
        {
            var param = InitParams();
            var tagsParam = Utils.ToTagParams(tags);
            param.Add("tag_token_list", tagsParam);
            return RestfulPost(GV.BATCHDELTAG, param);
        }

        /// <summary>
        /// 删除群发推送任务的离线消息(/v2/push/delete_offline_msg)
        /// </summary>
        /// <param name="pushId">任务ID</param>
        /// <exception cref="ArgumentException">pushId 不能为空或者 null</exception>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> DeleteOfflineAsync(string pushId)
        {
            TryThrowArgumentException(nameof(pushId), pushId);

            var param = InitParams();
            param.Add("push_id", pushId);
            return RestfulPost(GV.DELETEOFFLINEPUSH, param);
        }

        /// <summary>
        /// 删除应用中某个account映射的某个token(/v2/application/del_app_account_tokens)
        /// </summary>
        /// <param name="account">设备别名</param>
        /// <param name="deviceToken">device_token</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> DeleteAccountTokensAsync(string account, string deviceToken)
        {
            TryThrowArgumentException("account 或 deviceToken 不能为空或者null.", account, deviceToken);

            var param = InitParams();
            param.Add("account", account);
            param.Add("device_token", deviceToken);
            return RestfulPost(GV.DELETETOKENOFACCOUNT, param);
        }

        /// <summary>
        /// 删除应用中某account映射的所有token
        /// URL: /v2/application/del_app_account_all_tokens
        /// </summary>
        /// <param name="account">设备别名(账号)</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> DeleteAccountTokensAsync(string account)
        {
            TryThrowArgumentException(nameof(account), account);

            var param = InitParams();
            param.Add("account", account);
            return RestfulPost(GV.DELETEALLTOKENSOFACCOUNT, param);
        }

        /// <summary>
        /// 创建多条推送Id
        /// URL: /v2/push/create_multipush
        /// </summary>
        /// <param name="msg">通知消息</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        public Task<string> CreateMultiPushAsync(Notification msg)
        {
            var param = InitParams(msg);
            return RestfulPost(GV.CREATEMULTIPUSH, param);
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="url">访问URL</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="param">http 参数</param>
        /// <returns>返回签名</returns>
        public string Signature(string url, string method, Dictionary<string, string> param)
        {
            var builder = new StringBuilder();
            builder.Append(method);
            builder.Append(url.Replace("http://", string.Empty));
            foreach (var item in param.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                builder.Append($"{item.Key}={item.Value}");
            }
            builder.Append(SecretKey);
            return Utils.Md5(builder.ToString());
        }

        /// <summary>
        /// Http 推送方法
        /// </summary>
        /// <param name="url">访问URL</param>
        /// <param name="param">http 参数</param>
        /// <returns>腾讯服务器返回内容(未格式化)</returns>
        protected async Task<string> RestfulPost(string url, Dictionary<string, string> param)
        {
            var sign = Signature(url, "POST", param);
            param.Add("sign", sign);
            using (var client = HttpHandler != null ? new HttpClient(HttpHandler) : new HttpClient())
            {
                var response = await client.PostAsync(url, new FormUrlEncodedContent(param));
                string content = await response.Content.ReadAsStringAsync();
                HttpCallback?.Invoke(url, param, content);
                return content;
            }
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns>通用参数</returns>
        protected Dictionary<string, string> InitParams(Notification msg = null)
        {
            if (Timestamp == 0)
            {
                Timestamp = (uint)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
            }
            uint time = ValidTime > 600 ? 600 : ValidTime;
            var param = new Dictionary<string, string>
            {
                { "access_id", AccessId },
                { "timestamp", Timestamp.ToString() },
                { "valid_time", time.ToString() }
            };
            if (msg != null)
            {
                param.Add("message", msg.ToJson());
                param.Add("expire_time", ExpireTime.ToString());
                if (msg is AndroidNotification)
                {
                    param.Add("message_type", msg.MessageType == MessageType.Notification ? "1" : "2");
                    param.Add("multi_pkg", "1");
                }
                else
                {
                    param.Add("message_type", "0");
                    param.Add("environment", IsDevelopment ? "2" : "1");
                }
            }
            return param;
        }

        /// <summary>
        ///  工具方法测试数据是否为空
        /// </summary>
        /// <param name="message">提示消息</param>
        /// <param name="values">需要检查的值</param>
        protected void TryThrowArgumentException(string message, params string[] values)
        {
            if (values.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException(message);
            }
        }
    }
}