using System;
using Meta.ORM.Common;
using Meta.ORM.Entities;
using Meta.ORM.Model;

namespace Meta.ORM.Data
{
    [InMemoryPersistence]
    [RelationalPersistence]
    public abstract class BaseDictionaryEntity : IEntity 
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