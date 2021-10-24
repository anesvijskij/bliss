using System;
using Bliss.ORM.Entities;
using Bliss.ORM.Model;
using Bliss.ORM.Sql;

namespace Bliss.ORM.Query
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