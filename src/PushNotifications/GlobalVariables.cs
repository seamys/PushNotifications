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

// ReSharper disable InconsistentNaming
namespace PushNotifications
{
    /// <summary>
    ///     全局变量设置
    /// </summary>
    internal class GlobalVariables
    {
        #region 推送地址

        /// <summary>
        /// 单个设备推送地址
        /// URL: /v2/push/single_device
        /// </summary>
        public const string PUSHSINGLEDEVICE = "http://openapi.xg.qq.com/v2/push/single_device";

        /// <summary>
        /// 单个帐号推送地址
        /// URL: /v2/push/single_account
        /// </summary>
        public const string PUSHSINGLEACCOUNT = "http://openapi.xg.qq.com/v2/push/single_account";

        /// <summary>
        /// 批量帐号推送地址
        /// URL: /v2/push/account_list
        /// </summary>
        public const string PUSHACCOUNTLIST = "http://openapi.xg.qq.com/v2/push/account_list";

        /// <summary>
        /// 全量设备推送地址
        /// URL: /v2/push/all_device
        /// </summary>
        public const string PUSHALLDEVICE = "http://openapi.xg.qq.com/v2/push/all_device";

        /// <summary>
        /// 标签推送地址
        /// URL: /v2/push/tags_device
        /// </summary>
        public const string PUSHTAGS = "http://openapi.xg.qq.com/v2/push/tags_device";

        /// <summary>
        /// 查询群发消息发送状态
        /// URL: /v2/push/get_msg_status
        /// </summary>
        public const string QUERYPUSHSTATUS = "http://openapi.xg.qq.com/v2/push/get_msg_status";

        /// <summary>
        /// 查询应用覆盖的设备数（token总数）
        /// URL: /v2/application/get_app_device_num
        /// </summary>
        public const string QUERYDEVICECOUNT = "http://openapi.xg.qq.com/v2/application/get_app_device_num";

        /// <summary>
        /// 查询应用设置的标签
        /// URL: /v2/tags/query_app_tags
        /// </summary>
        public const string QUERYTAGS = "http://openapi.xg.qq.com/v2/tags/query_app_tags";

        /// <summary>
        /// 取消尚未触发的定时群发任务
        /// URL: /v2/push/cancel_timing_task
        /// </summary>
        public const string CANCELTIMINGPUSH = "http://openapi.xg.qq.com/v2/push/cancel_timing_task";

        /// <summary>
        /// 批量设置标签
        /// URL: /v2/tags/batch_set
        /// </summary>
        public const string BATCHSETTAG = "http://openapi.xg.qq.com/v2/tags/batch_set";

        /// <summary>
        /// 批量删除标签
        /// URL: /v2/tags/batch_del
        /// </summary>
        public const string BATCHDELTAG = "http://openapi.xg.qq.com/v2/tags/batch_del";

        /// <summary>
        /// 查询应用的某个设备上设置的标签
        /// URL: /v2/tags/query_token_tags
        /// </summary>
        public const string QUERYTOKENTAGS = "http://openapi.xg.qq.com/v2/tags/query_token_tags";

        /// <summary>
        /// 查询应用某个标签下关联的设备数
        /// URL: /v2/tags/query_tag_token_num
        /// </summary>
        public const string QUERYTAGTOKENNUM = "http://openapi.xg.qq.com/v2/tags/query_tag_token_num";

        /// <summary>
        /// 创建批量消息
        /// URL: /v2/push/create_multipush
        /// </summary>
        public const string CREATEMULTIPUSH = "http://openapi.xg.qq.com/v2/push/create_multipush";

        /// <summary>
        /// 批量帐号推送
        /// URL: /v2/push/create_multipush
        /// </summary>
        public const string PUSHACCOUNTLISTMULTIPLE = "http://openapi.xg.qq.com/v2/push/account_list_multiple";

        /// <summary>
        /// 批量设备推送
        /// URL: /device_list_multiple
        /// </summary>
        public const string PUSHDEVICELISTMULTIPLE = "http://openapi.xg.qq.com/v2/push/device_list_multiple";

        /// <summary>
        /// 查询应用的某个token的信息（查看是否有效）
        /// URL: /v2/application/get_app_token_info
        /// </summary>
        public const string QUERYINFOOFTOKEN = "http://openapi.xg.qq.com/v2/application/get_app_token_info";

        /// <summary>
        /// 查询应用某帐号映射的token（查看帐号-token对应关系）
        /// URL: /v2/application/get_app_account_tokens
        /// </summary>
        public const string QUERYTOKENSOFACCOUNT = "http://openapi.xg.qq.com/v2/application/get_app_account_tokens";

        /// <summary>
        /// 删除应用中某个account映射的某个token
        /// URL: /v2/application/del_app_account_tokens
        /// </summary>
        public const string DELETETOKENOFACCOUNT = "http://openapi.xg.qq.com/v2/application/del_app_account_tokens";

        /// <summary>
        /// 删除应用中某account映射的所有token
        /// URL: /v2/application/del_app_account_all_tokens
        /// </summary>
        public const string DELETEALLTOKENSOFACCOUNT =
            "http://openapi.xg.qq.com/v2/application/del_app_account_all_tokens";

        /// <summary>
        /// 删除群发推送任务的离线消息
        /// URL: /v2/push/delete_offline_msg
        /// </summary>
        public const string DELETEOFFLINEPUSH = "http://openapi.xg.qq.com/v2/push/delete_offline_msg";

        #endregion
    }
}