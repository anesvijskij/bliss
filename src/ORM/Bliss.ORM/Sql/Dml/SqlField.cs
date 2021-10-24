namespace Bliss.ORM.Sql.Dml
{
    public class SqlField : Statement
    {
        public SqlField(string field)
        {
            Field = field;
        }

        public string Field { get; }
    }
}
