namespace Application.Caching.Constants
{
    public static class CacheKeys
    {
        public static string PagedEvents(int pageNumber, int pageSize) => $"events:paged:{pageNumber}:{pageSize}";

        public static string SeatLockKey(Guid seatId) => $"lock:seats:{seatId}";
    }
}
