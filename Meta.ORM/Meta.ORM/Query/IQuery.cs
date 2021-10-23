using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Meta.ORM.Entities;
using Meta.ORM.Expressions;

namespace Meta.ORM.Query
{
    /// <summary>
    /// Represents a query object to RDMS, which result should be translated to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of domain entities.</typeparam>
    public interface IQuery<T>
    {
        /// <summary>
        /// Show, that all entities in query should be returned only once.
        /// </summary>
        /// <returns>Query object.</returns>
        IQuery<T> Distinct();

        /// <summary>
        /// Indicates, that all loaded entities from query should be updated with data from db, even if they already have changes.
        /// </summary>
        /// <returns>Query object.</returns>
        IQuery<T> ForseRefresh();

        /// <summary>
        /// Sets id list of entities, that should be loaded from db. This method is more usefull, than using "in (val1,...,valN)".
        /// </summary>
        /// <param name="ids">List of identifiers.</param>
        /// <returns>Query object.</returns>
        IQuery<T> IdList(IEnumerable<object> ids);

        /// <summary>
        /// This method allows to include in query property path, which is expressed as a <param name="propertyExpression"></param>.
        /// Supports only syntax as Property1. ... PropertyN, or calls of <see cref="IEntity.GetPropertyValue(string)"/>.
        /// </summary>
        /// <typeparam name="TProperty">Type of property path.</typeparam>
        /// <param name="propertyExpression">Expression for property path.</param>
        /// <returns>Query object.</returns>
        IQuery<T> Include<TProperty>(Expression<Func<T, TProperty>> propertyExpression);

        /// <summary>
        /// Call of this method includes all immediate list (array) properties of <see cref="T"/> in query.
        /// </summary>
        /// <returns>Query object.</returns>
        IQuery<T> IncludeAllListProperties();

        /// <summary>
        /// Call of this method includes all immediate sub properties of referenced properties of <see cref="T"/> in query.
        /// </summary>
        /// <returns>Query object.</returns>
        IQuery<T> IncludeAllReferences();

        /// <summary>
        /// Includes calculated column/property in query.
        /// </summary>
        /// <param name="calculationExpression">Expression for calculation.</param>
        /// <param name="name">Unique name for calculated column.</param>
        /// <returns>Query object.</returns>
        IQuery<T> IncludeCalculation(MetaExpression calculationExpression, string name);

        /// <summary>
        /// Includes calculated column/property in query.
        /// </summary>
        /// <typeparam name="TProperty">Type of calculated property.</typeparam>
        /// <param name="calculationExpression">Function for calculated property.</param>
        /// <param name="name">Unique name for calculated column.</param>
        /// <returns>Query object.</returns>
        IQuery<T> IncludeCalculation<TProperty>(Expression<Func<T, TProperty>> calculationExpression, string name);

        /// <summary>
        /// Includes calculated column/property in query.
        /// </summary>
        /// <param name="calculationExpression">Expression for calculation.</param>
        /// <param name="name">Unique name for calculated column.</param>
        /// <returns>Query object.</returns>
        IQuery<T> IncludeCalculation<TProperty>(MetaExpression<TProperty> calculationExpression, string name);

        /// <summary>
        /// This method allows to include in query content  of <see cref="IEnumerable{TProperty}"/> property as sub-query.
        /// Supports only syntax as Property, or calls on <see cref="IEntity.GetPropertyValue(string)"/>.
        /// </summary>
        /// <typeparam name="TProperty">Type of property.</typeparam>
        /// <param name="propertyExpression">Expression for property path.</param>
        /// <param name="listConfig">Configuration for sub-query.</param>
        /// <returns>Query object.</returns>
        IQuery<T> IncludeList<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> propertyExpression, Action<Query<TProperty>>? listConfig = null);

        /// <summary>
        /// Link current query with another query of <see cref="TJoin"/> objects with specific condition.
        /// </summary>
        /// <typeparam name="TJoin">Type of current query.</typeparam>
        /// <typeparam name="TResult">Type of linked query.</typeparam>
        /// <param name="join">Linked query.</param>
        /// <param name="condition">Condition for linking two queries.</param>
        /// <param name="projection">Function for transforming linked result to new object of type <see cref="TResult"/>.</param>
        /// <returns>Query object of type <see cref="TResult"/>.</returns>
        IQuery<TResult> Join<TJoin, TResult>(Query<TJoin> join, Expression<Func<T, TJoin, bool>> condition, Expression<Func<T, IEnumerable<TJoin>, TResult>> projection);

        /// <summary>
        /// Link current query with another query of <see cref="TJoin"/> objects with specific condition.
        /// </summary>
        /// <typeparam name="TJoin">Type of current query.</typeparam>
        /// <typeparam name="TResult">Type of linked query.</typeparam>
        /// <param name="join">Linked query.</param>
        /// <param name="condition">Condition for linking two queries.</param>
        /// <param name="projection">Function for transforming linked result to new object of type <see cref="TResult"/>.</param>
        /// <returns>Query object of type <see cref="TResult"/>.</returns>
        IQuery<TResult> Join<TJoin, TResult>(Query<TJoin> join, MetaExpression<bool> condition, Expression<Func<T, IEnumerable<TJoin>, TResult>> projection);

        /// <summary>
        /// Sets top/limit + offset for query.
        /// </summary>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>Query object</returns>
        IQuery<T> Offset(int offset, int? limit);

        /// <summary>
        /// Call of this method specifies, than entities only of current type <see cref="T"/> should be extracted from database.
        /// Inherited entities should be ignored.
        /// </summary>
        /// <returns>Query object.</returns>
        IQuery<T> Only();

        /// <summary>
        /// Sets ascending sorting order for query.
        /// </summary>
        /// <typeparam name="TProperty">Type of sorted expression.</typeparam>
        /// <param name="propertyExpression">Property or expression, which is used for sorting.</param>
        /// <returns>Query object.</returns>
        IOrderedQuery<T> OrderBy<TProperty>(Expression<Func<T, CalculatedPropertiesCollection, TProperty>> propertyExpression);

        /// <summary>
        /// Sets ascending sorting order for query.
        /// </summary>
        /// <typeparam name="TProperty">Type of sorted expression.</typeparam>
        /// <param name="propertyExpression">Property or expression, which is used for sorting.</param>
        /// <returns>Query object.</returns>
        IOrderedQuery<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> propertyExpression);

        /// <summary>
        /// Sets descending sorting order for query.
        /// </summary>
        /// <typeparam name="TProperty">Type of sorted expression.</typeparam>
        /// <param name="propertyExpression">Property or expression, which is used for sorting.</param>
        /// <returns>Query object.</returns>
        IOrderedQuery<T> OrderByDescending<TProperty>(Expression<Func<T, CalculatedPropertiesCollection, TProperty>> propertyExpression);

        /// <summary>
        /// Sets descending sorting order for query.
        /// </summary>
        /// <typeparam name="TProperty">Type of sorted expression.</typeparam>
        /// <param name="propertyExpression">Property or expression, which is used for sorting.</param>
        /// <returns>Query object.</returns>
        IOrderedQuery<T> OrderByDescending<TProperty>(Expression<Func<T, TProperty>> propertyExpression);

        /// <summary>
        /// This method initiates recursion query, where each step of recursion should be linked with the previous state with the additional condition in <see cref="stepExpression"/>.
        /// </summary>
        /// <param name="stepExpression">Link condition between steps of query.</param>
        /// <returns>Query object.</returns>
        IQuery<T> RecursionStep(Expression<Func<T, T, bool>> stepExpression);

        /// <summary>
        /// This method initiates recursion query, where each step of recursion should be linked with the previous state with the additional condition in <see cref="stepExpression"/>.
        /// </summary>
        /// <param name="stepExpression">Link condition between steps of query.</param>
        /// <returns>Query object.</returns>
        IQuery<T> RecursionStep(MetaExpression<bool> stepExpression);

        /// <summary>
        /// Sets projection of each entity of type <see cref="T"/> to entities of type <see cref="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The end type, in which initial entities should be transformed.</typeparam>
        /// <param name="projection">Function for transforming linked result to new object of type <see cref="TResult"/>.</param>
        /// <returns>Query object of type <see cref="TResult"/>.</returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, CalculatedPropertiesCollection, TResult>> projection);

        /// <summary>
        /// Sets projection of each entity of type <see cref="T"/> to entities of type <see cref="TResult"/>.
        /// </summary>
        /// <typeparam name="TResult">The end type, in which initial entities should be transformed.</typeparam>
        /// <param name="projection">Function for transforming linked result to new object of type <see cref="TResult"/>.</param>
        /// <returns>Query object of type <see cref="TResult"/>.</returns>
        IQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> projection);

        /// <summary>
        /// Starts enumeration of objects in query - it will immediately initiate a SQL query to DB.
        /// </summary>
        /// <returns>The sequence to type <see cref="T"/></returns>
        IEnumerable<T> AsEnumerable();

        /// <summary>
        /// Returns list of objects, initiating a SQL query to DB.
        /// </summary>
        /// <returns>List of objects of type <see cref="T"/>, which fully satisfies all parameters of query.</returns>
        IEnumerable<T> ToList();

        /// <summary>
        /// Start async query to DB, which should returns a sequence to type <see cref="T"/>.
        /// </summary>
        /// <returns>List of objects of type <see cref="T"/>, which fully satisfies all parameters of query.</returns>
        Task<IEnumerable<T>> ToListAsync();

        /// <summary>
        /// Sets condition for entities in query. If there is already another condition, than <param name="condition"></param> would be merged with original condition
        /// with and operator.
        /// </summary>
        /// <param name="condition">Condition for entities.</param>
        /// <returns>Query object.</returns>
        IQuery<T> Where(Expression<Func<T, bool>> condition);

        /// <summary>
        /// Sets condition for entities in query. If there is already another condition, than <param name="condition"></param> would be merged with original condition
        /// with and operator.
        /// </summary>
        /// <param name="condition">Condition for entities.</param>
        /// <returns>Query object.</returns>
        IQuery<T> Where(Expression<Func<T, CalculatedPropertiesCollection, bool>> condition);

        /// <summary>
        /// Sets condition for entities in query. If there is already another condition, than <param name="condition"></param> would be merged with original condition
        /// with and operator.
        /// </summary>
        /// <param name="condition">Condition for entities.</param>
        /// <returns>Query object.</returns>
        IQuery<T> Where(MetaExpression<bool> condition);
    }
}