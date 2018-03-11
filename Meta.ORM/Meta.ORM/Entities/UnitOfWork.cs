using System;
using System.Collections.Generic;
using System.Text;
using Meta.ORM.Model;
using Meta.ORM.Sql;

namespace Meta.ORM.Entities
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IModel _model;
        private readonly ISqlBuilder _sqlBuilder;
        private readonly IDbProvider _dbProvider;

        public UnitOfWork(IModel model, ISqlBuilder sqlBuilder, IDbProvider dbProvider)
        {
            _model = model;
            _sqlBuilder = sqlBuilder;
            _dbProvider = dbProvider;
        }

        public void Dispose()
        {
            
        }

        public IEntitiesRepository GetRepository(Type entityType)
        {
            return new EntitiesRepository(_model, this, _sqlBuilder, _dbProvider, entityType);
        }

        public IEntitiesRepository GetRepository(IEntityType entityType)
        {
            return new EntitiesRepository(_model, this, _sqlBuilder, _dbProvider, entityType.SystemType);
        }

        public IGenericEntitiesRepository<T> GetRepository<T>() where T : class, IEntity, new()
        {
            return new GenericEntitiesRepository<T>(_model, this, _sqlBuilder, _dbProvider);
        }

        public bool SaveChanges()
        {
            return true;
        }

        public void Attach<T>(T entity)
        {
            
        }
    }
}
