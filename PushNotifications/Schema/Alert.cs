using System.Collections.Generic;

namespace PushNotifications.Schema
{
    /// <summary>
    ///  Notification Payload
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        public Alert()
        {
            Body = null;
            ActionLocalizedKey = null;
            LocalizedKey = null;
            LocalizedArgs = new List<object>();
        }

        /// <summary>
        /// ֪ͨ��Ϣ��
        /// </summary>
        public string Body
        {
            get;
            set;
        }

        /// <summary>
        /// Action ��ť ���ػ� Key
        /// </summary>
        public string ActionLocalizedKey
        {
            get;
            set;
        }

        /// <summary>
        /// ���ػ� Key
        /// </summary>
        public string LocalizedKey
        {
            get;
            set;
        }

        /// <summary>
        /// ���ػ������б�
        /// </summary>
        public List<object> LocalizedArgs
        {
            get;
            set;
        }
        /// <summary>
        /// ��Ӳ���
        /// </summary>
        /// <param name="values"></param>
        public void AddLocalizedArgs(params object[] values)
        {
            this.LocalizedArgs.AddRange(values);
        }

        /// <summary>
        /// ȷ�� Alert �Ƿ񾯱��ǿյ�
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
