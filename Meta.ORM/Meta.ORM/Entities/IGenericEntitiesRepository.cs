namespace Meta.ORM.Entities
{
    public interface IGenericEntitiesRepository<T>
    {
        Query.Query<T> Query();

        T Create();
    }
}
