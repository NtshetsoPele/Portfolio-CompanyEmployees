namespace Shared.RequestFeatures;

public class PagedList<T> : List<T>
{
    public MetaData MetaData { get; }

    public PagedList(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
    {
        MetaData = new MetaData
        {
            TotalCount  = totalCount,
            PageSize    = pageSize,
            CurrentPage = currentPage,
            TotalPages  = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        AddRange(items);
    }

    /*
    Paging logic can be pushed to the repository layer
    public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
    */

    public static PagedList<T> ToPagedList(
        IEnumerable<T> source, int totalCount, int currentPage, int pageSize)
    {
        return new PagedList<T>(source, totalCount, currentPage, pageSize);
    }
}