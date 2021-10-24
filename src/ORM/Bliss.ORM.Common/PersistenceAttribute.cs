using System;

namespace Bliss.ORM.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public abstract class PersistenceAttribute : Attribute
    {
    }

    public class RelationalPersistenceAttribute : PersistenceAttribute
    {
        
    }

    public class UnstructuredPersistenceAttribute : PersistenceAttribute
    {
        
    }
    
    public class GraphPersistenceAttribute : PersistenceAttribute
    {
        
    }
    
    public class SpatialPersistenceAttribute : PersistenceAttribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class InMemoryPersistenceAttribute : Attribute
    {
        
    }
}