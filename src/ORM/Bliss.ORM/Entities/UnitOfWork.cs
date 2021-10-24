using System;
using System.Collections.Generic;
using System.Text;
using Bliss.ORM.Model;
using Bliss.ORM.Sql;

namespace Bliss.ORM.Entities
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

        public IEntitiesRepository GetRepository(Type type)
        {
            Type dqType = typeof(GenericEntitiesRepository<>).MakeGenericType(type);
            return (IEntitiesRepository)Activator.CreateInstance(dqType, _model, this, _sqlBuilder, _dbProvider)!;
        }

        public IEntitiesRepository GetRepository(IEntityType entityType)
        {
            Type dqType = typeof(GenericEntitiesRepository<>).MakeGenericType(entityType.SystemType);
            return (IEntitiesRepository)Activator.CreateInstance(dqType, _model, this, _sqlBuilder, _dbProvider)!;
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
