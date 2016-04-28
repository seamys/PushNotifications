using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PushNotifications.Schema
{
    /// <summary>
    /// 动作
    /// </summary>
    public class Action
    {
        /// <summary>
        /// 动作类型，1打开activity或app本身，2打开浏览器，3打开Intent
        /// </summary>
        public int ActionType { get; set; }
        /// <summary>
        /// 动作
        /// </summary>
        public string Activity { get; set; }
        /// <summary>
        /// activity属性，只针对action_type=1的情况
        /// </summary>
        public Dictionary<string, int> ActivityAttribute { get; set; }
        /// <summary>
        /// url：打开的url，confirm是否需要用户确认    
        /// </summary>
        public Browser Browser { get; set; }
        /// <summary>
        /// intent
        /// </summary>
        public string Intent { get; set; }
        /// <summary>
        /// 内容转化
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