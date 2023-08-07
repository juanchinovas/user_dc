namespace Application.Common.Models;

public class Pagination<T>
{
    public IEnumerable<T> Items { get; private set; }
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; } = 10;
    public int PageCount { get; private set; }
    public int TotalItems { get; private set; }

    public static Task<Pagination<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult(
            new Pagination<T>
            {
                Items = items,
                TotalItems = count,
                PageIndex = pageNumber,
                PageSize = pageSize,
                PageCount = (int)Math.Round((decimal)count / pageSize)
            }
        );
    }

    public static Pagination<T> Create(IEnumerable<T> source, int pageNumber, int pageSize, int pageCount)
    {return new Pagination<T>
        {
            Items = source,
            TotalItems = pageCount,
            PageIndex = pageNumber,
            PageSize = pageSize,
            PageCount = (int)Math.Round((decimal)pageCount / pageSize)
        };
    }
}
