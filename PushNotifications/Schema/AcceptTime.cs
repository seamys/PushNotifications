using System;

namespace PushNotifications.Schema
{
    /// <summary>
    /// 发送时间段
    /// </summary>
    public class AcceptTime
    {
        /// <summary>
        /// 开始时间段
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// 结束时间段
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// 转化string方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{{ \"start\":{{ \"hour\":\"{Start.Hour}\"}},{{\"min\":\"{Start.Minute}\"}},\"end\":{{ \"hour\":\"{End.Hour}\"}},{{\"min\":\"{End.Minute}\"}}}}";
        }
    }
}