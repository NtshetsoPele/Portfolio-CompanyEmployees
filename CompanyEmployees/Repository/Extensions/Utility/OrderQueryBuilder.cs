namespace Repository.Extensions.Utility;

public static class OrderQueryBuilder
{
    public static string CreateOrderQuery<T>(string orderByQueryString)
    {
        IEnumerable<string> orderParams = SplitOrderByStrings(orderByQueryString);
        PropertyInfo[] propertyInfos = GetBindingProperties<T>();
        var orderQueryBuilder = new StringBuilder();
        foreach (string param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                continue;
            }
            PropertyInfo? objectProperty = TryGetParamProperty(param, propertyInfos);
            if (objectProperty == null)
            {
                continue;
            }
            AddOrderByDirectionToOrderQuery(param, orderQueryBuilder, objectProperty);
        }
        return GenerateOrderByQuery(orderQueryBuilder);
    }

    private static IEnumerable<string> SplitOrderByStrings(string orderByQueryString) =>
        orderByQueryString.Trim().Split(separator: ',');

    private static PropertyInfo[] GetBindingProperties<T>() =>
        typeof(T).GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.Instance);

    private static PropertyInfo? TryGetParamProperty(string param, IEnumerable<PropertyInfo> propertyInfos)
    {
        var propertyFromQueryName = param.Split(separator: " ")[0]; // Spans?
        return propertyInfos.FirstOrDefault((PropertyInfo pi) =>
            pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
    }

    private static void AddOrderByDirectionToOrderQuery(string param, StringBuilder builder, MemberInfo member)
    {
        var direction = param.EndsWith(" desc") ? "descending" : "ascending";
        builder.Append($"{member.Name} {direction}, ");
    }

    private static string GenerateOrderByQuery(StringBuilder orderQueryBuilder) =>
        orderQueryBuilder.ToString().TrimEnd(',', ' ');
}