using System.Collections.Generic;

namespace PushNotifications.Schema
{
    /// <summary>
    ///  Notification Payload
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Alert()
        {
            Body = null;
            ActionLocalizedKey = null;
            LocalizedKey = null;
            LocalizedArgs = new List<object>();
        }

        /// <summary>
        /// 通知消息体
        /// </summary>
        public string Body
        {
            get;
            set;
        }

        /// <summary>
        /// Action 按钮 本地化 Key
        /// </summary>
        public string ActionLocalizedKey
        {
            get;
            set;
        }

        /// <summary>
        /// 本地化 Key
        /// </summary>
        public string LocalizedKey
        {
            get;
            set;
        }

        /// <summary>
        /// 本地化参数列表
        /// </summary>
        public List<object> LocalizedArgs
        {
            get;
            set;
        }
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="values"></param>
        public void AddLocalizedArgs(params object[] values)
        {
            this.LocalizedArgs.AddRange(values);
        }

        /// <summary>
        /// 确定 Alert 是否警报是空的
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (!string.IsNullOrEmpty(Body)
                    || !string.IsNullOrEmpty(ActionLocalizedKey)
                    || !string.IsNullOrEmpty(LocalizedKey)
                    || (LocalizedArgs != null && LocalizedArgs.Count > 0))
                    return false;
                else
                    return true;
            }
        }
    }
}
