using System;
using Bliss.ORM.Common;
using Bliss.ORM.Entities;
using Bliss.ORM.Model;

namespace Bliss.ORM.Data
{
    [SpatialPersistence]
    public abstract class BaseSpatialEntity : IEntity
    {
        public int Id { get; }
        public object GetPropertyValue(string propertyName)
        {
            throw new NotImplementedException();
        }

        public T GetPropertyValue<T>(string propertyName)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyValue(IProperty property)
        {
            throw new NotImplementedException();
        }

        public T GetPropertyValue<T>(IProperty propertyName)
        {
            throw new NotImplementedException();
        }

        public T GetPropertyValue<T>(IProperty<T> property)
        {
            throw new NotImplementedException();
        }
    }
}