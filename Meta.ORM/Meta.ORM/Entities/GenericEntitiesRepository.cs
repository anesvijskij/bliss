using System;
using System.Collections.Generic;
using System.Text;
using Meta.ORM.Model;
using Meta.ORM.Query;
using Meta.ORM.Sql;

namespace Meta.ORM.Entities
{
    public class GenericEntitiesRepository<T> : IGenericEntitiesRepository<T> where T : class, IEntity, new()
    {
        private readonly IModel _model;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISqlBuilder _sqlBuilder;
        private readonly IDbProvider _dbProvider;

        public GenericEntitiesRepository(IModel model, IUnitOfWork unitOfWork, ISqlBuilder sqlBuilder, IDbProvider dbProvider)
        {
            _model = model;
            _unitOfWork = unitOfWork;
            _sqlBuilder = sqlBuilder;
            _dbProvider = dbProvider;
        }

        public Query<T> Query()
        {
            return new Query<T>(_model, _unitOfWork, _sqlBuilder, _dbProvider);
        }

        public T Create()
        {
            return new T();
        }
    }
}
