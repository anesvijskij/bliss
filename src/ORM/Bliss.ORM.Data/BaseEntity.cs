﻿using System;
using Bliss.ORM.Common;
using Bliss.ORM.Entities;
using Bliss.ORM.Model;

namespace Bliss.ORM.Data
{
    [RelationalPersistence]
    public abstract class BaseEntity : IEntity
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