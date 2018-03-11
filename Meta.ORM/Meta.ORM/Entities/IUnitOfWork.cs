using System;
using System.Collections.Generic;
using System.Text;
using Meta.ORM.Model;

namespace Meta.ORM.Entities
{
    public interface IUnitOfWork : IDisposable
    {
        IEntitiesRepository GetRepository(Type entityType);

        IEntitiesRepository GetRepository(IEntityType entityType);

        IGenericEntitiesRepository<T> GetRepository<T>() where T : class, IEntity, new();

        bool SaveChanges();

        void Attach<T>(T entity);
    }
}
