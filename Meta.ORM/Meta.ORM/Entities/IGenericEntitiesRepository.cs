using System;
using System.Collections.Generic;
using System.Text;

namespace Meta.ORM.Entities
{
    public interface IGenericEntitiesRepository<T>
    {
        Query.Query<T> Query();

        T Create();

        
    }
}
