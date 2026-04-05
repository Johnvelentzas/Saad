
using Models;

namespace Producion_Line_Manager.Helpers
{
    public class RequestParameters
    {
        public int Page;
        public int PageSize;
        public List<FilterType> Filters = new();
        public SortType SortType;
        public SearchType? SearchType;
        public string? SearchValue;

        public bool HasFilters => Filters.Count > 0;
        public bool HasSearch => !string.IsNullOrEmpty(SearchValue) && SearchType != null;
        public bool IsSearchType => HasFilters && HasSearch;

        public RequestParameters(
            List<FilterType>? filters = null,
            SearchType? searchType = null,
            string? searchValue = null,
            int page = 1, 
            int pageSize = 100, 
            SortType sortType = SortType.IdDecending)
        {
            if (filters != null)
            {
                Filters = filters;
            }
            SearchType = searchType;
            SearchValue = searchValue;
            Page = page;
            PageSize = pageSize;
            SortType = sortType;
        }

        public string BuildURI(string uri = "")
        {
            if (IsSearchType)
            {
                uri += $"/search?";
            }
            else
            {
                uri += $"?";
            }
            foreach (var filter in Filters)
            {
                uri += $"filters={filter}&";
            }
            if (HasSearch)
            {
                uri += $"searchtype={SearchType}&searchvalue={SearchValue}&";
            }
            uri += $"page={Page}&pagesize={PageSize}&sort={SortType}";
            return uri;
        }
    }
}
