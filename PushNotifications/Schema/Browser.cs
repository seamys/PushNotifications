namespace PushNotifications.Schema
{
    /// <summary>
    /// 打开浏览器
    /// </summary>
    public class Browser
    {
        /// <summary>
        /// url：打开的url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// confirm是否需要用户确认
        /// </summary>
        public int Confirm { get; set; }
    }
}