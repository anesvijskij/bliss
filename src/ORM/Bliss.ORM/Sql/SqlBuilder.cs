using Bliss.ORM.Sql.Dml.Select;

namespace Bliss.ORM.Sql
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
