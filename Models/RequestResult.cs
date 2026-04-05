using System.Text.Json.Serialization;

namespace Models
{
    public interface IRequestResult
    {
        int TotalCount { get; }
        int TotalPages { get; }
        int CurrentPage { get; }
        IEnumerable<IEntity> Items { get; }
    }
    public class RequestResult<T> : IRequestResult
        where T : class, IEntity
    {
        public int TotalCount { get; set; } = 1;

        public int TotalPages { get; set; } = 1;

        public int CurrentPage { get; set; } = 1;

        public IEnumerable<T> Items { get; set; }

        IEnumerable<IEntity> IRequestResult.Items => Items;

        [JsonConstructor]
        public RequestResult(IEnumerable<T> items, int totalCount, int totalPages, int currentPage)
        {
            Items = items;
            TotalCount = totalCount;
            TotalPages = totalPages;
            CurrentPage = currentPage;
        }

        public RequestResult(T item)
        {
            Items = new List<T> { item };
        }

        public RequestResult()
        {
            Items = new List<T>();
        }

    }
}
