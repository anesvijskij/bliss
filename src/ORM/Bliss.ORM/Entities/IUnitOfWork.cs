﻿using System;
using Bliss.ORM.Model;

namespace Bliss.ORM.Entities
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
