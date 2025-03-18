#pragma warning disable CS8618
namespace DataAccess.Entities
{
    public class PagedCollection<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
