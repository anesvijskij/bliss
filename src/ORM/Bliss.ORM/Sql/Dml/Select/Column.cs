namespace Bliss.ORM.Sql.Dml.Select
{
    public abstract class Column
    {
        protected readonly Select Select;

        public Column(Select select)
        {
            Select = select;
            
        }
    }
}