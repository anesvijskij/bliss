using Meta.ORM.Sql.Dml.Select;

namespace Meta.ORM.Sql
{
    public class SqlBuilder : ISqlBuilder
    {
        //public Select Select()
        //{
        //    return new Select();
        //}

        public GenericSelect<T> Select<T>()
        {
            return new GenericSelect<T>();
        }
    }
}
