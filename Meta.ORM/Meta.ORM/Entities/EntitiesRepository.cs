using System;
using Meta.ORM.Model;
using Meta.ORM.Query;
using Meta.ORM.Sql;

namespace Meta.ORM.Entities
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
            return null;
        }
    }
}
