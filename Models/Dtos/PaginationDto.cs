namespace CodeQuestBackend.Models.Dtos;

public class PaginationDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PaginatedResultDto<T>
{
    public ICollection<T> Data { get; set; } = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}