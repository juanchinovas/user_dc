using Core.Models;
using Domain.Entities;

namespace Application.Common.Models;

public class Pagination<T> : DataResum<T>
{
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; } = 10;
    public int PageCount { get; private set; }

    public static Pagination<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new Pagination<T>
        {
            Items = items,
            TotalItems = count,
            PageIndex = pageNumber,
            PageSize = pageSize,
            PageCount = (int)Math.Round((decimal)count / pageSize)
        };
    }

    public static Pagination<T> Create(DataResum<T> source, int pageNumber, int pageSize)
    {
        var currentPage = (pageNumber - 1) * pageSize;
        var items = source.Items;
        if (currentPage < source.Items.Count())
        {
            items = source.Items.Skip(currentPage).Take(pageSize);
        }

        return new Pagination<T>
        {
            Items = items,
            TotalItems = source.TotalItems,
            PageIndex = pageNumber,
            PageSize = pageSize,
            PageCount = (int)Math.Round((decimal)source.TotalItems / pageSize)
        };
    }
}
