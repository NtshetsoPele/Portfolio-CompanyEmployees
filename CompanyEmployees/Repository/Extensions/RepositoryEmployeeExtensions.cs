namespace Repository.Extensions;

public static class RepositoryEmployeeExtensions
{
    public static IQueryable<Employee> FilterEmployees(
        this IQueryable<Employee> employees, uint minAge, uint maxAge)
    {
        return employees.Where((Employee e) => e.Age >= minAge && e.Age <= maxAge);
    }

    // Simplified searching example.
    public static IQueryable<Employee> Search(this IQueryable<Employee> employees, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return employees;
        }
        var lowerCaseName = searchTerm.Trim().ToLower();
        return employees.Where((Employee e) => e.Name!.ToLower().Contains(lowerCaseName));
    }

    public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return employees.OrderBy((Employee e) => e.Name);
        }
        string orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);
        if (string.IsNullOrWhiteSpace(orderQuery))
        {
            return employees.OrderBy((Employee e) => e.Name);
        }
        return employees.OrderBy(orderQuery);
    }
}