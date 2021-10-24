using System;
using Bliss.ORM.Model;
using Bliss.ORM.Query;
using Bliss.ORM.Sql;

namespace Bliss.ORM.Entities
{
    public class EntitiesRepository : IEntitiesRepository
    {
        private readonly IModel _model;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISqlBuilder _sqlBuilder;
        private readonly IDbProvider _dbProvider;
        private readonly Type _type;

        public EntitiesRepository(IModel model, IUnitOfWork unitOfWork, ISqlBuilder sqlBuilder, IDbProvider dbProvider,
            Type type)
        {
            _model = model;
            _unitOfWork = unitOfWork;
            _sqlBuilder = sqlBuilder;
            _dbProvider = dbProvider;
            _type = type;
        }

        public Query<IEntity> Query()
        {
            return new DynamicQuery(_model, _unitOfWork, _sqlBuilder, _dbProvider, _type);
        }

        public IEntity Create()
        {
            return Activator.CreateInstance(_type) as IEntity ?? throw new InvalidOperationException();
        }
    }
}