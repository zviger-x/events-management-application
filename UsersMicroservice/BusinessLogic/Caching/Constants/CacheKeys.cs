namespace BusinessLogic.Caching.Constants
{
    public static class CacheKeys
    {
        public static string UserById(Guid id) => $"users:{id}";
        public static string AllUsers => "users:all";
        public static string PagedUsers(int pageNumber, int pageSize) => $"users:paged:{pageNumber}:{pageSize}";
        public static string UserJwtToken(Guid userId) => $"tokens:{userId}";
    }
}
