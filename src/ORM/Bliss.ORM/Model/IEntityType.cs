using System;
using System.Collections.Generic;

namespace Bliss.ORM.Model
{
    public interface IEntityType
    {
        string Name { get; }

        string Description { get; }

        IModel Model { get; }

        IEntityType Base { get; }

        IRuntimeProperty GetProperty(string name, bool inherit = true);

        IEnumerable<IRuntimeProperty> GetProperties(bool inherit);

        Type SystemType { get; }
    }
}