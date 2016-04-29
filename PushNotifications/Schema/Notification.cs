using System.Collections.Generic;

namespace PushNotifications.Schema
{
    /// <summary>
    /// 通知基类
    /// </summary>
    public abstract class Notification
    {
        /// <summary>
        /// 自定义参数
        /// </summary>
        public Dictionary<string, object> CustomItems
        {
            get;
            protected set;
        }
        /// <summary>
        /// 添加自定义参数
        /// </summary>
        /// <param name="key">自定义Key</param>
        /// <param name="values">自定义值</param>
        public void AddCustom(string key, object values)
        {
            if (values != null)
                this.CustomItems.Add(key, values);
        }
        /// <summary>
        /// 转化成Json 
        /// </summary>
        /// <returns></returns>
        public abstract string ToJson();
        /// <summary>
        /// 推送消息类型
        /// </summary>
        public MessageType MessageType { get; set; }
        /// <summary>
        /// 重写ToString 方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToJson();
        }
    }
}