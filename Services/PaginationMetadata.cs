namespace CityInfo.API.Services
{
    public class PaginationMetadata
    {
        public int PageSize { get; set; }

        public int TotalPageCount { get; set; }

        public int TotalItemCount { get; set; }

        public int CurrentPage { get; set; }

        public PaginationMetadata(int pageSize, int currentPage, int totalItemCount)
        {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        }
    }
}