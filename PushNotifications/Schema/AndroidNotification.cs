using System;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PushNotifications.Schema
{
    public class AndroidNotification : Notification
    {

        public AndroidNotification(string title, string content)
        {
            Title = title;
            Content = content;
        }
        public string Title { get; set; }
        public string Content { get; set; }
        public uint? NotificationId { get; set; }
        public uint BuilderId { get; set; }
        public uint? Ring { get; set; }
        public string RingRaw { get; set; }
        public uint? Vibrate { get; set; }
        public uint? Lights { get; set; }
        public uint? Clearable { get; set; }
        public uint? IconType { get; set; }
        public string IconRes { get; set; }
        public string SmallIcon { get; set; }
        public uint? StyleId { get; set; }
        public Action Action { get; set; }
        public override string ToJson()
        {
            JObject custom = new JObject();
            JObject json = new JObject
            {
                ["title"] = Title,
                ["content"] = Content,
                ["builder_id"] = BuilderId
            };
            if (NotificationId.HasValue)
                json["n_id"] = NotificationId;
            if (Ring.HasValue)
            {
                json["ring"] = Ring;
                if (!string.IsNullOrWhiteSpace(RingRaw))
                    json["ring_raw"] = RingRaw;
            }
            if (Action != null)
            {
                var action = new JObject
                {
                    ["action_type"] = Action.ActionType,
                    ["activity"] = Action.Activity
                };
                if (Action.ActivityAttribute != null)
                    action["aty_attr"] = new JObject(Action.ActivityAttribute);

                if (Action.Browser != null)
                {
                    action["browser"] = new JObject
                    {
                        ["url"] = Action.Browser.Url,
                        ["confirm"] = Action.Browser.Confirm
                    };
                }
                json["action"] = action;
            }
            if (Vibrate.HasValue)
                json["vibrate"] = Vibrate;
            if (Lights.HasValue)
                json["lights"] = Lights;
            if (Clearable.HasValue)
                json["clearable"] = Clearable;
            if (IconType.HasValue)
            {
                json["icon_type"] = IconType;
                if (!string.IsNullOrWhiteSpace(IconRes))
                    json["icon_res"] = IconRes;
                if (!string.IsNullOrWhiteSpace(SmallIcon))
                    json["small_icon"] = SmallIcon;
            }
            if (StyleId.HasValue)
                json["style_id"] = StyleId;
            foreach (string key in this.CustomItems.Keys)
            {
                if (CustomItems[key].Length == 1)
                    custom[key] = new JValue(this.CustomItems[key][0]);
                else if (CustomItems[key].Length > 1)
                    custom[key] = new JArray(this.CustomItems[key]);
            }
            json["accept_time"] = new JArray(AcceptTime);
            json["custom_content"] = new JArray(custom);
            string rawString = json.ToString(Newtonsoft.Json.Formatting.None, null);
            StringBuilder encodedString = new StringBuilder();
            foreach (char c in rawString)
            {
                if (c < 32 || c > 127)
                    encodedString.Append($"\\u{Convert.ToUInt32(c):x4}");
                else
                    encodedString.Append(c);
            }
            return rawString;// encodedString.ToString();
        }
        public override string ToString()
        {
            return ToJson();
        }
    }
}