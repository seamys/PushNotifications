namespace PushNotifications.Schema
{
    public class PushResult
    {
        public int RetCode { get; set; }
        public string ErrMsg { get; set; }
        public ResultStatus Result { get; set; }
    }
}
