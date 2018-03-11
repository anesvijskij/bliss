using Meta.ORM.Model;

namespace Meta.ORM.Entities
{
    public interface IEntity
    {
        int Id { get; }

        object GetPropertyValue(string propertyName);

        T GetPropertyValue<T>(string propertyName);

        object GetPropertyValue(IProperty property);

        T GetPropertyValue<T>(IProperty propertyName);

        T GetPropertyValue<T>(IProperty<T> property);
    }
}