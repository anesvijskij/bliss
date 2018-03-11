using System;
using System.Linq.Expressions;

namespace Meta.ORM.Sql.Dml.Select
{
    public class GenericSelect<T> : Select
    {
        public GenericSelect(string alias = null, string schema = null) : base(typeof(T).Name, alias ?? typeof(T).Name, schema)
        {

        }

        public new GenericSelect<T> Column(Statement expression, string alias = null)
        {
            AppendColumn(new StatementColumn(expression, this, alias));
            return this;
        }

        public GenericSelect<T> Column<TProperty>(Expression<Func<T, TProperty>> columnDefinition, string alias = null)
        {
            if (columnDefinition.Body is MemberExpression memberExpression)
            {
                if (memberExpression.Expression is MemberExpression)
                    throw new ArgumentException("Property paths are not allowed");
                AppendColumn(new StatementColumn(new SqlField(memberExpression.Member.Name), this, alias));
                return this;
            }

            if (columnDefinition.Body is ConstantExpression contantExpression)
            {
                AppendColumn(new StatementColumn(new Constant(contantExpression.Value), this, alias));
                return this;
            }

            throw new ArgumentException("Invalid Column");
        }

        public new GenericSelect<T> AllColumns()
        {
            AppendColumn(new StarColumn(this));
            return this;
        }
    }
}