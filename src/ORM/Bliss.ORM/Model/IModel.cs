using System;
using System.Collections.Generic;
using System.Text;

namespace Bliss.ORM.Model
{
    public interface IModel
    {
        IEntityType GetEntityType(string name);

        IEnumerable<IEntityType> GetEntityTypes();

        IEntityType GetEntityType<T>();

        bool HasEntityType(string name);

        IEnumerable<IEntityType> GetInheritedEntityTypes(string typeName, bool recursive = false);
    }
}