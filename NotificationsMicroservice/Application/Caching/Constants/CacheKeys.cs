namespace Application.Caching.Constants
{
    internal class CacheKeys
    {
        public static string FailedNotification(Guid inCacheId) => $"notification:failed:{inCacheId}";

        public static string FailedNotificationsSet => "notification:failed:set";
    }
}
