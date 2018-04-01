using System;
using System.Linq.Expressions;

namespace Meta.ORM.Query
{
    public interface IOrderedQuery<T> : IQuery<T>
    {
        /// <summary>
        /// Adds ascending sorting order for query.
        /// </summary>
        /// <typeparam name="TProperty">Type of sorted expression.</typeparam>
        /// <param name="propertyExpression">Property or expression, which is used for sorting.</param>
        /// <returns>Query object.</returns>
        IOrderedQuery<T> ThenBy<TProperty>(Expression<Func<T, CalculatedPropertiesCollection, TProperty>> propertyExpression);

        /// <summary>
        /// Adds ascending sorting order for query.
        /// </summary>
        /// <typeparam name="TProperty">Type of sorted expression.</typeparam>
        /// <param name="propertyExpression">Property or expression, which is used for sorting.</param>
        /// <returns>Query object.</returns>
        IOrderedQuery<T> ThenBy<TProperty>(Expression<Func<T, TProperty>> propertyExpression);

        /// <summary>
        /// Adds descending sorting order for query.
        /// </summary>
        /// <typeparam name="TProperty">Type of sorted expression.</typeparam>
        /// <param name="propertyExpression">Property or expression, which is used for sorting.</param>
        /// <returns>Query object.</returns>
        IOrderedQuery<T> ThenByDescending<TProperty>(Expression<Func<T, CalculatedPropertiesCollection, TProperty>> propertyExpression);

        /// <summary>
        /// Adds descending sorting order for query.
        /// </summary>
        /// <typeparam name="TProperty">Type of sorted expression.</typeparam>
        /// <param name="propertyExpression">Property or expression, which is used for sorting.</param>
        /// <returns>Query object.</returns>
        IOrderedQuery<T> ThenByDescending<TProperty>(Expression<Func<T, TProperty>> propertyExpression);
    }
}