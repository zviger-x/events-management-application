namespace Application.Caching.Constants
{
    public static class CacheKeys
    {
        public static string EventById(Guid id) => $"events:{id}";
        public static string PagedEvents(int pageNumber, int pageSize) => $"events:paged:{pageNumber}:{pageSize}";
    }
}
