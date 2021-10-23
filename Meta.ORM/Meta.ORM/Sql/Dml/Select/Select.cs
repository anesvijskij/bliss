namespace Meta.ORM.Sql.Dml.Select
{
    public class Select : Statement
    {
        private readonly string _tableName;
        private readonly string? _alias;
        private readonly string? _schema;

        private readonly ColumnList _columns;

        public Select(string tableName, string? alias = null, string? schema = null)
        {
            _tableName = tableName;
            _alias = alias;
            _schema = schema;
            _columns = new ColumnList();
        }

        protected void AppendColumn(Column column)
        {
            _columns.Add(column);
        }

        public Select Column(Statement expression, string? alias = null)
        {
            AppendColumn(new StatementColumn(expression, this, alias));
            return this;
        }

        public Select Column(string columnName, string? alias = null)
        {
            AppendColumn(new StatementColumn(new SqlField(columnName), this, alias));
            return this;
        }

        public Select AllColumns()
        {
            AppendColumn(new StarColumn(this));
            return this;
        }
    }
}
