namespace Bliss.ORM.Entities
{
    public interface IGenericEntitiesRepository<T> where  T:IEntity
    {
        Query.Query<T> Query();

        T Create();
    }
}
