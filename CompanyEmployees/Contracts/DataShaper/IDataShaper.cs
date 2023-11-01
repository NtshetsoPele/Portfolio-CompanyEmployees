namespace Contracts.DataShaper;

public interface IDataShaper<in T>
{
    // Represents an object whose members can be dynamically added and removed at run time.
    IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString);
    ExpandoObject ShapeData(T entity, string fieldsString);
}