public class Pagination
{
  public static PaginationResult getPagination(int? page, int? limit, int count)
  {
    if (limit == null)
    {
      limit = 10;
    }
    if (page == null)
    {
      page = 1;
    }
    int totalPages = (int)Math.Ceiling((decimal)count / (decimal)limit);
    return new PaginationResult
    {
      limit = (int)limit,
      offset = ((int)page - 1) * (int)limit,
      page = (int)page,
      totalPages = totalPages,
      totalResults = count
    };
  }

  public class PaginationResult
  { 
    public int limit { get; set; } = 10;
    public int offset { get; set; } = 0;
    public int page { get; set; } = 1;
    public int totalPages { get; set; } = 1;
    public int totalResults { get; set; } = 0;
  }
}
