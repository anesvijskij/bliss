namespace Bliss.ORM.Sql.Dml.Select
{
    public class StatementColumn : Column
    {
        public StatementColumn(Statement expression, Select select, string? alias) : base(select)
        {
            Statement = new StatementWithAlias(expression, alias);
        }

        public StatementWithAlias Statement { get; }
    }
}