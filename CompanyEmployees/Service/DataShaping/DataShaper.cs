namespace Service.DataShaping;

public class DataShaper<T> : IDataShaper<T> where T : class
{
    public PropertyInfo[] Properties { get; set; }

    public DataShaper()
    {
        Properties = typeof(T).GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.Instance);
    }

    public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString)
    {
        IEnumerable<PropertyInfo> requiredProperties = GetRequiredProperties(fieldsString);
        return FetchData(entities, requiredProperties.ToList());
    }

    public ExpandoObject ShapeData(T entity, string fieldsString)
    {
        IEnumerable<PropertyInfo> requiredProperties = GetRequiredProperties(fieldsString);
        return FetchDataForEntity(entity, requiredProperties);
    }

    private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
    {
        var requiredProperties = new List<PropertyInfo>();
        if (!string.IsNullOrWhiteSpace(fieldsString))
        {
            string[] fields = SplitFields();
            foreach (string field in fields)
            {
                PropertyInfo? property = TryGetMatchingProperty(field);
                if (property != null)
                {
                    requiredProperties.Add(property);
                }
            }
        }
        else
        {
            requiredProperties = Properties.ToList();
        }
        return requiredProperties;

        #region Nested_Helpers

        string[] SplitFields()
        {
            return fieldsString.Split(separator: ',', StringSplitOptions.RemoveEmptyEntries);
        }

        PropertyInfo? TryGetMatchingProperty(string field)
        {
            return Properties.FirstOrDefault((PropertyInfo pi) => pi.Name.Equals(value: field.Trim(),
                StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }

    private static IEnumerable<ExpandoObject> FetchData(
        IEnumerable<T> entities, IList<PropertyInfo> requiredProperties)
    {
        var shapedData = new List<ExpandoObject>();
        foreach (T entity in entities)
        {
            var shapedObject = FetchDataForEntity(entity, requiredProperties);
            shapedData.Add(shapedObject);
        }
        return shapedData;
    }

    private static ExpandoObject FetchDataForEntity(
        T entity, IEnumerable<PropertyInfo> requiredProperties)
    {
        var shapedObject = new ExpandoObject();
        foreach (PropertyInfo property in requiredProperties)
        {
            var objectPropertyValue = property.GetValue(entity);
            shapedObject.TryAdd(property.Name, objectPropertyValue);
        }
        return shapedObject;
    }
}