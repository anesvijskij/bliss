using System;
using Meta.ORM.Entities;
using Meta.ORM.Model;
using Meta.ORM.Sql;

namespace Meta.ORM.Query
{
    public class DynamicQuery : Query<IEntity>
    {
        private readonly Type _type;

        internal DynamicQuery(IModel model, IUnitOfWork unitOfWork, ISqlBuilder sqlBuilder, IDbProvider dbProvider,
            Type type) : base(model, unitOfWork, sqlBuilder, dbProvider)
        {
            _type = type;
        }

        protected override Type GetQueryType()
        {
            return _type;
        }
    }
}