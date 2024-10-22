using System.Net;

namespace smERP.SharedKernel.Bases;

public class PaginatedResult<T>
{
    //public PaginatedResult(List<T> data)
    //{
    //    Data = data;
    //}
    public List<T> Data { get; set; }

    public PaginatedResult(List<T> data = default, int count = 0, int page = 1, int pageSize = 10)
    {
        Data = data;
        CurrentPage = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
    }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;

}

