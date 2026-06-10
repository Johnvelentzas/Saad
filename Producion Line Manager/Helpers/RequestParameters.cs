
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
        public bool IsSearchType => HasSearch || HasIdInRoute;
        public bool HasIdInRoute = false;

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

        public static RequestParameters GetOne()
        {
            RequestParameters par = new();
            par.PageSize = 1;
            return par;
        }

        public virtual string BuildURI(string baseRoute = "")
        {
            var segments = new List<string>();

            // Handle Filters
            foreach (var filter in Filters)
            {
                segments.Add($"filters={filter}");
            }

            // Handle Search
            if (HasSearch)
            {
                segments.Add($"searchtype={SearchType}");
                segments.Add($"searchvalue={Uri.EscapeDataString(SearchValue ?? "")}"); // Good practice for spaces/special characters
            }

            // Handle Pagination and Sorting
            segments.Add($"page={Page}");
            segments.Add($"pagesize={PageSize}");
            segments.Add($"sort={SortType}");

            // Combine segments cleanly
            string queryString = string.Join("&", segments);
            string endpointType = HasSearch ? "/search" : "";

            return $"{baseRoute}{endpointType}?{queryString}";
        }
    }

    public class ModelsRequestParameters : RequestParameters
    {
        public int? BrandId;
        public int? CategoryId;
        public ModelsRequestParameters(
            List<FilterType>? filters = null,
            SearchType? searchType = null,
            string? searchValue = null,
            int page = 1, 
            int pageSize = 100, 
            SortType sortType = SortType.IdDecending,
            int? brandId = null,
            int? categoryId = null) : base(filters, searchType, searchValue, page, pageSize, sortType)
        {
            BrandId = brandId;
            CategoryId = categoryId;
        }
        public override string BuildURI(string baseRoute = "")
        {
            var segments = new List<string>();

            // Handle Filters
            foreach (var filter in Filters)
            {
                segments.Add($"filters={filter}");
            }

            // Handle Search
            if (HasSearch)
            {
                segments.Add($"searchtype={SearchType}");
                segments.Add($"searchvalue={Uri.EscapeDataString(SearchValue ?? "")}"); // Good practice for spaces/special characters
            }

            // Handle Pagination and Sorting
            segments.Add($"page={Page}");
            segments.Add($"pagesize={PageSize}");
            segments.Add($"sort={SortType}");

            // Combine segments cleanly
            string queryString = string.Join("&", segments);
            string endpointType = "/search_by_brand_and_category";

            var baseUri = $"{baseRoute}{endpointType}?{queryString}";

            var extraSegments = new List<string>();

            if (BrandId != null)
            {
                extraSegments.Add($"brandId={BrandId}");
            }

            if (CategoryId != null)
            {
                extraSegments.Add($"categoryId={CategoryId}");
            }

            if (extraSegments.Count > 0)
            {
                string extraQuery = string.Join("&", extraSegments);
                return $"{baseUri}&{extraQuery}";
            }

            return baseUri;
        }
    }
}
