using Bliss.ORM.Sql.Dml.Select;

namespace Bliss.ORM.Sql
{
    public interface ISqlBuilder
    {
        GenericSelect<T> Select<T>();
    }
}