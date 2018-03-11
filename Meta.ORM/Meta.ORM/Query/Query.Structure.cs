using System.Linq.Expressions;
using Meta.ORM.Expressions;

namespace Meta.ORM.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public partial class Query<T>
    {
        private readonly HashSet<QueryProperty> _propertiesToLoad = new HashSet<QueryProperty>();

        private readonly CalculatedPropertiesCollection _calculatedProperties = new CalculatedPropertiesCollection();
        private bool _only;
        private MetaExpression<bool> _condition;
        private int _offset;
        private int? _limit;
        private bool _forseRefresh;
        private bool _distinct;
        private IEnumerable<object> _ids;

        private QueryProperty AddPropertyPathToPropertyCollection(IEnumerable<MemberInfo> path)
        {
            var currentCollection = _propertiesToLoad;
            QueryProperty includedProperty = null;
            foreach (var memberInfo in path)
            {
                includedProperty = currentCollection.FirstOrDefault(it => it.Member == memberInfo);
                if (includedProperty == null)
                {
                    includedProperty = new QueryProperty { Member = memberInfo };
                    currentCollection.Add(includedProperty);
                }

                currentCollection = includedProperty.InnerProperties;
            }

            return includedProperty;
        }

        private ListQueryProperty<TProperty> AddListPropertyToPropertyCollection<TProperty>(MemberInfo member)
        {
            if (_propertiesToLoad.Any(it => it.Member == member))
            {
                throw new ArgumentException($"Property {member.Name} already added to query.");
            }

            var queryProperty = new ListQueryProperty<TProperty>
                                {
                                    Member = member,
                                    ListQuery = new Query<TProperty>(_model,
                                        _unitOfWork, _sqlBuilder, _dbProvider)
                                };
            _propertiesToLoad.Add(queryProperty);
            return queryProperty;
        }

        private MetaExpression<T1> ConvertExpression<T1>(Expression<Func<T, T1>> condition)
        {
            var visitor = new LinqToMetaExpressionVisitor(GetQueryType());
            visitor.Visit(condition);
            return visitor.MetaExpression as MetaExpression<T1>;
        }

        private MetaExpression<T1> ConvertExpression<T1>(Expression<Func<T, CalculatedPropertiesCollection, T1>> condition)
        {
            var visitor = new LinqToMetaExpressionVisitor(GetQueryType());
            visitor.Visit(condition);
            return visitor.MetaExpression as MetaExpression<T1>;
        }

        protected virtual Type GetQueryType()
        {
            return typeof(T);
        }
    }
}