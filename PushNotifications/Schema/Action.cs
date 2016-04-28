using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PushNotifications.Schema
{
    /// <summary>
    /// ����
    /// </summary>
    public class Action
    {
        /// <summary>
        /// �������ͣ�1��activity��app����2���������3��Intent
        /// </summary>
        public int ActionType { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Activity { get; set; }
        /// <summary>
        /// activity���ԣ�ֻ���action_type=1�����
        /// </summary>
        public Dictionary<string, int> ActivityAttribute { get; set; }
        /// <summary>
        /// url���򿪵�url��confirm�Ƿ���Ҫ�û�ȷ��    
        /// </summary>
        public Browser Browser { get; set; }
        /// <summary>
        /// intent
        /// </summary>
        public string Intent { get; set; }
        /// <summary>
        /// ����ת��
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JObject obj = new JObject
            {
                ["action_type"] = ActionType,
            };
            if (!string.IsNullOrEmpty(Intent))
                obj["intent"] = Intent;
            if (string.IsNullOrWhiteSpace(Activity))
                obj["activity"] = Activity;
            if (ActivityAttribute != null && ActivityAttribute.Count > 0)
            {
                obj["aty_attr"] = new JObject();
                foreach (var item in ActivityAttribute)
                {
                    obj["aty_attr"][item.Key] = item.Value;
                }
            }
            if (Browser != null)
            {
                obj["browser"] = new JObject(new
                {
                    url = Browser.Url,
                    confirm = Browser.Confirm
                });
            }
            return obj.ToString(Newtonsoft.Json.Formatting.None, null);
        }
    }
}