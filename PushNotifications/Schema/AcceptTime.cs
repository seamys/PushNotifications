using System;

namespace PushNotifications.Schema
{
    /// <summary>
    /// ����ʱ���
    /// </summary>
    public class AcceptTime
    {
        /// <summary>
        /// ��ʼʱ���
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// ����ʱ���
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// ת��string����
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{{ \"start\":{{ \"hour\":\"{Start.Hour}\"}},{{\"min\":\"{Start.Minute}\"}},\"end\":{{ \"hour\":\"{End.Hour}\"}},{{\"min\":\"{End.Minute}\"}}}}";
        }
    }
}