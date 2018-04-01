using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Meta.ORM.Entities;
using Meta.ORM.Expressions;
using System.Linq;
using System.Reflection;
using Meta.ORM.Model;
using Meta.ORM.Sql;

namespace Meta.ORM.Query
{
    /// <summary>
    /// Represents a query object to RDMS, which result should be translated to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of domain entities.</typeparam>
    public partial class Query<T> : IQuery<T>, IOrderedQuery<T>
    {
        private readonly IModel _model;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISqlBuilder _sqlBuilder;
        private readonly IDbProvider _dbProvider;

        internal Query(IModel model, IUnitOfWork unitOfWork, ISqlBuilder sqlBuilder, IDbProvider dbProvider)
        {
            _model = model;
            _unitOfWork = unitOfWork;
            _sqlBuilder = sqlBuilder;
            _dbProvider = dbProvider;
        }

        /// <inheritdoc />
        public IQuery<T> Include<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            // Simply sequence of properties
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                var propertyPath = GetPropertyPathFromExpression(memberExpression);
                AddPropertyPathToPropertyCollection(propertyPath);
                return this;
            }

            // call of GetPropertyValue method of IEntity
            if (propertyExpression.Body is MethodCallExpression methodCallExpression &&
                methodCallExpression.Object is ParameterExpression &&
                methodCallExpression.Method.Name == "GetPropertyValue" && methodCallExpression.Arguments.Count == 1)
            {
                var argument = methodCallExpression.Arguments[0];
                if (argument is ConstantExpression constantExpression && constantExpression.Value is string stringValue)
                {
                    var path = stringValue.Split('.');

                    var currentType = GetQueryType();
                    var propertyPath = new List<MemberInfo>();
                    foreach (var pathItem in path)
                    {
                        var itemPropertyInfo = currentType.GetProperty(pathItem);
                        if (itemPropertyInfo == null)
                            throw new ArgumentException("Invalid Property");

                        propertyPath.Add(itemPropertyInfo);
                        currentType = itemPropertyInfo.PropertyType;
                    }

                    AddPropertyPathToPropertyCollection(propertyPath);
                    return this;
                }

                if (argument is MemberExpression argumentMemberExpression && argumentMemberExpression.Type.GetInterfaces().Contains(typeof(IProperty)) )
                {
                    var memberName = argumentMemberExpression.Member.Name;
                    if (argumentMemberExpression.Expression is ConstantExpression argumentConstantExpression)
                    {
                        var fieldInfo = argumentConstantExpression.Type.GetField(memberName);
                        var property = (IProperty)fieldInfo.GetValue(argumentConstantExpression.Value);

                        var path = property.Name.Split('.');

                        var currentType = GetQueryType();
                        var propertyPath = new List<MemberInfo>();
                        foreach (var pathItem in path)
                        {
                            var itemPropertyInfo = currentType.GetProperty(pathItem);
                            if (itemPropertyInfo == null)
                                throw new ArgumentException("Invalid Property");

                            propertyPath.Add(itemPropertyInfo);
                            currentType = itemPropertyInfo.PropertyType;
                        }

                        AddPropertyPathToPropertyCollection(propertyPath);
                        return this;
                    }
                }
            }

            throw new ArgumentException("Invalid Property");
        }

        /// <inheritdoc />
        public IQuery<T> IncludeList<TProperty>(
            Expression<Func<T, IEnumerable<TProperty>>> propertyExpression,
            Action<Query<TProperty>> listConfig = null)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                var member = memberExpression.Member;
                if (member.MemberType != MemberTypes.Property)
                    throw new ArgumentException("Only Properties Allowed in Include statements");

                var queryProperty = AddListPropertyToPropertyCollection<TProperty>(member);
                listConfig?.Invoke(queryProperty.ListQuery);
                return this;
            }

            if (propertyExpression.Body is MethodCallExpression methodCallExpression &&
                methodCallExpression.Object is ParameterExpression &&
                methodCallExpression.Method.Name == "GetPropertyValue" && methodCallExpression.Arguments.Count == 1)
            {
                var argument = methodCallExpression.Arguments[0];
                if (argument is ConstantExpression constantExpression && constantExpression.Value is string stringValue)
                {
                    var path = stringValue.Split('.');
                    if (path.Length > 1)
                        throw new ArgumentException("Invalid Property");

                    var currentProperty = GetQueryType().GetProperty(stringValue);

                    if (currentProperty == null)
                        throw new ArgumentException("Invalid Property");

                    var queryProperty = AddListPropertyToPropertyCollection<TProperty>(currentProperty);
                    listConfig?.Invoke(queryProperty.ListQuery);
                    return this;
                }

                if (argument is MemberExpression argumentMemberExpression && argumentMemberExpression.Type.GetInterfaces().Contains(typeof(IProperty)))
                {
                    var memberName = argumentMemberExpression.Member.Name;
                    if (argumentMemberExpression.Expression is ConstantExpression argumentConstantExpression)
                    {
                        var fieldInfo = argumentConstantExpression.Type.GetField(memberName);
                        var property = (IProperty)fieldInfo.GetValue(argumentConstantExpression.Value);

                        var path = property.Name.Split('.');

                        if (path.Length > 1)
                            throw new ArgumentException("Invalid Property");

                        var currentProperty = GetQueryType().GetProperty(property.Name);

                        if (currentProperty == null)
                            throw new ArgumentException("Invalid Property");

                        var queryProperty = AddListPropertyToPropertyCollection<TProperty>(currentProperty);
                        listConfig?.Invoke(queryProperty.ListQuery);
                        return this;
                    }
                }
            }

            throw new ArgumentException("Invalid Property");
        }

        /// <inheritdoc />
        public IQuery<T> IncludeCalculation<TProperty>(Expression<Func<T, TProperty>> calculationExpression, string name)
        {
            var calcProperty =
                new CalculatedProperty<TProperty>
                {
                    Name = name,
                    Expression = ConvertExpression(calculationExpression)
                };

            _calculatedProperties.AddProperty(name, calcProperty);
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> IncludeCalculation<TProperty>(MetaExpression<TProperty> calculationExpression, string name)
        {
            var calcProperty =
                new CalculatedProperty<TProperty>
                {
                    Name = name,
                    Expression = calculationExpression
                };

            _calculatedProperties.AddProperty(name, calcProperty);
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> IncludeCalculation(MetaExpression calculationExpression, string name)
        {
            var calcProperty =
                new CalculatedProperty
                {
                    Name = name,
                    Expression = calculationExpression
                };

            _calculatedProperties.AddProperty(name, calcProperty);
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> IncludeAllReferences()
        {
            foreach (var propertyInfo in typeof(T).GetProperties().Where(it=>it.PropertyType.IsClass && !it.PropertyType.ContainsGenericParameters))
            {
                AddPropertyPathToPropertyCollection(new []{ propertyInfo });
            }
            
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> IncludeAllListProperties()
        {
            //foreach (var propertyInfo in typeof(T).GetProperties().Where(it => it.PropertyType.GetGenericTypeDefinition() == ty))
            //{
            //    AddPropertyPathToPropertyCollection(new[] { propertyInfo });
            //}
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> Only()
        {
            _only = true;
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> Where(Expression<Func<T, bool>> condition)
        {
            MetaExpression<bool> metaCondition = ConvertExpression(condition);
            _condition = _condition == null ? metaCondition : _condition.And(metaCondition);
            return this;
        }

        ///// <summary>
        ///// Sets condition for entities in query. If there is already another condition, than <param name="condition"></param> would be merged with original condition
        ///// with and operator.
        ///// </summary>
        ///// <param name="condition">Condition for entities.</param>
        ///// <returns>Query object.</returns>
        //public Query<T> Where(MetaExpression condition)
        //{
        //    _condition = _condition == null ? condition : _condition.And(condition);
        //    return this;
        //}

        /// <inheritdoc />
        public IQuery<T> Where(MetaExpression<bool> condition)
        {
            _condition = _condition == null ? condition : _condition.And(condition);
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> Where(Expression<Func<T, CalculatedPropertiesCollection, bool>> condition)
        {
            MetaExpression<bool> metaCondition = ConvertExpression<bool>(condition);
            _condition = _condition == null ? metaCondition : _condition.And(metaCondition);
            return this;
        }

        /// <inheritdoc />
        public IOrderedQuery<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return this;
        }

        /// <inheritdoc />
        public IOrderedQuery<T> OrderBy<TProperty>(Expression<Func<T, CalculatedPropertiesCollection, TProperty>> propertyExpression)
        {
            return this;
        }

        /// <inheritdoc />
        public IOrderedQuery<T> OrderByDescending<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return this;
        }

        /// <inheritdoc />
        public IOrderedQuery<T> OrderByDescending<TProperty>(Expression<Func<T, CalculatedPropertiesCollection, TProperty>> propertyExpression)
        {
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> Offset(int offset, int? limit)
        {
            _offset = offset;
            _limit = limit;
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> ForseRefresh()
        {
            _forseRefresh = true;
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> Distinct()
        {
            _distinct = true;
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> IdList(IEnumerable<object> ids)
        {
            _ids = ids;
            return this;
        }

        /// <inheritdoc />
        public IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> projection)
        {
            return new Query<TResult>(_model, _unitOfWork, _sqlBuilder, _dbProvider);
        }

        /// <inheritdoc />
        public IEnumerable<T> AsEnumerable()
        {
            return this.ToList();
        }

        /// <inheritdoc />
        public IQuery<TResult> Select<TResult>(Expression<Func<T, CalculatedPropertiesCollection, TResult>> projection)
        {
            return new Query<TResult>(_model, _unitOfWork, _sqlBuilder, _dbProvider);
        }

        /// <inheritdoc />
        public IQuery<TResult> Join<TJoin, TResult>(Query<TJoin> join, Expression<Func<T, TJoin, bool>> condition, Expression<Func<T, IEnumerable<TJoin>, TResult>> projection)
        {
            return new Query<TResult>(_model, _unitOfWork, _sqlBuilder, _dbProvider);
        }

        /// <inheritdoc />
        public IQuery<TResult> Join<TJoin, TResult>(Query<TJoin> join, MetaExpression<bool> condition,
            Expression<Func<T, IEnumerable<TJoin>, TResult>> projection)
        {
            return new Query<TResult>(_model, _unitOfWork, _sqlBuilder, _dbProvider);
        }

        /// <inheritdoc />
        public IQuery<T> RecursionStep(Expression<Func<T, T, bool>> stepExpression)
        {
            return this;
        }

        /// <inheritdoc />
        public IQuery<T> RecursionStep(MetaExpression<bool> stepExpression)
        {
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<T> ToList()
        {
            return new List<T>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> ToListAsync()
        {
            return await Task.Run(() => new List<T>());
        }

        /// <inheritdoc />
        public IOrderedQuery<T> ThenBy<TProperty>(Expression<Func<T, CalculatedPropertiesCollection, TProperty>> propertyExpression)
        {
            return this;
        }

        /// <inheritdoc />
        public IOrderedQuery<T> ThenBy<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return this;
        }

        /// <inheritdoc />
        public IOrderedQuery<T> ThenByDescending<TProperty>(Expression<Func<T, CalculatedPropertiesCollection, TProperty>> propertyExpression)
        {
            return this;
        }

        /// <inheritdoc />
        public IOrderedQuery<T> ThenByDescending<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return this;
        }

        private IEnumerable<MemberInfo> GetPropertyPathFromExpression(MemberExpression memberExpression)
        {
            var member = memberExpression.Member;
            if (member.MemberType != MemberTypes.Property)
                throw new ArgumentException("Only Properties Allowed in Include statements");

            if (memberExpression.Expression is MemberExpression innerMemberExpression)
            {
                return GetPropertyPathFromExpression(innerMemberExpression).Append(member);
            }

            if (memberExpression.Expression != null && !(memberExpression.Expression is ParameterExpression))
            {
                throw new ArgumentException("Only Properties Allowed in Include statements");
            }

            return new List<MemberInfo> { member };
        }
    }
}