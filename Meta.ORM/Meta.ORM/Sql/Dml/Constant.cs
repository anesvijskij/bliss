namespace Meta.ORM.Sql.Dml
{
    public class Constant : Statement
    {
        public Constant(object value)
        {
            Value = value;
        }

        public object Value { get; }
    }
}
