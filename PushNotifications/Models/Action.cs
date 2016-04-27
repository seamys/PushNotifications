using System.Collections.Generic;

namespace PushNotifications.Models
{
    public class Action
    {
        public int ActionType { get; set; }
        public string Activity { get; set; }
        public Dictionary<string, int> ActivityAttribute { get; set; }
        public Browser Browser { get; set; }
        public string Intent { get; set; }
    }
}