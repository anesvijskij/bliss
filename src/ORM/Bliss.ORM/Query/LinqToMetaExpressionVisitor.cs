using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using Bliss.ORM.Entities;
using Bliss.ORM.Expressions;
using Bliss.ORM.Model;

namespace Bliss.ORM.Query
{
    public class LinqToMetaExpressionVisitor : ExpressionVisitor
    {
        private readonly Type _currentType;
        private MetaExpression? _expression;

        public LinqToMetaExpressionVisitor(Type currentType)
        {
            _currentType = currentType;
        }

        public MetaExpression? MetaExpression => _expression;

        /// <inheritdoc />
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.Equal)
            {
                Visit(node.Left);
                var leftExpression = _expression;
                Visit(node.Right);
                var rightExpression = _expression;

                _expression = leftExpression.Equal(rightExpression);
                return node;
            }
            if (node.NodeType == ExpressionType.And || node.NodeType == ExpressionType.AndAlso)
            {
                Visit(node.Left);
                var leftExpression = (MetaExpression<bool>)_expression;
                Visit(node.Right);
                var rightExpression = (MetaExpression<bool>)_expression;

                _expression = leftExpression.And(rightExpression);
                return node;
            }
            if (node.NodeType == ExpressionType.Or || node.NodeType == ExpressionType.OrElse)
            {
                Visit(node.Left);
                var leftExpression = (MetaExpression<bool>)_expression;
                Visit(node.Right);
                var rightExpression = (MetaExpression<bool>)_expression;

                _expression = leftExpression.Or(rightExpression);
                return node;
            }
            return base.VisitBinary(node);
        }

        /// <inheritdoc />
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is bool boolValue)
            {
                _expression = MetaExpression.Constant(boolValue);
            }
            else
            {
                _expression = MetaExpression.Constant(node.Value);
            }
            return base.VisitConstant(node);
        }

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            _expression = MetaExpression.Property(node.Member.Name);
            return base.VisitMember(node);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var argument = node.Arguments[0];
            if (argument is ConstantExpression constantExpression && constantExpression.Value is string stringValue)
            {
                //var path = stringValue.Split('.');

                //var currentType = node.Object.Type == typeof(IEntity) ? _currentType : node.Object.Type;
                //var propertyPath = new List<MemberInfo>();
                //foreach (var pathItem in path)
                //{
                //    var itemPropertyInfo = currentType.GetProperty(pathItem);
                //    if (itemPropertyInfo == null)
                //        throw new ArgumentException("Invalid Property");

                //    propertyPath.Add(itemPropertyInfo);
                //    currentType = itemPropertyInfo.PropertyType;
                //}

                _expression = MetaExpression.Property(stringValue);
                return node;
            }

            if (argument is MemberExpression argumentMemberExpression && ((IList) argumentMemberExpression.Type
                    .GetInterfaces()).Contains(typeof(IProperty)))
            {
                var memberName = argumentMemberExpression.Member.Name;
                if (argumentMemberExpression.Expression is ConstantExpression argumentConstantExpression)
                {
                    var fieldInfo = argumentConstantExpression.Type.GetField(memberName);
                    var property = (IProperty)fieldInfo.GetValue(argumentConstantExpression.Value);

                    var path = property.Name.Split('.');

                    var currentType = node.Object.Type == typeof(IEntity) ? _currentType : node.Object.Type;
                    var propertyPath = new List<MemberInfo>();
                    foreach (var pathItem in path)
                    {
                        var itemPropertyInfo = currentType.GetProperty(pathItem);
                        if (itemPropertyInfo == null)
                            throw new ArgumentException("Invalid Property");

                        propertyPath.Add(itemPropertyInfo);
                        currentType = itemPropertyInfo.PropertyType;
                    }

                    _expression = MetaExpression.Property(string.Join(".", propertyPath.Select(it => it.Name)));
                    return node;
                }
            }

            return base.VisitMethodCall(node);
        }
    }
}
