namespace Bliss.ORM.Sql.Dml.Select
{
    public class StatementWithAlias : Statement
    {
        public StatementWithAlias(Statement expression, string? alias)
        {
            Expression = expression;
            Alias = alias;
        }

        public Statement Expression { get; }

        public string? Alias { get; }
    }
}