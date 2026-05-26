namespace MES.Contracts.Common;

public class PagedQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Keyword { get; set; }
}
