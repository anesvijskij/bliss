using Meta.ORM.Sql.Dml.Select;

namespace Meta.ORM.Sql
{
    public interface ISqlBuilder
    {
        GenericSelect<T> Select<T>();
    }
}