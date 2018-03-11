using System.Collections.Generic;
using System.Reflection;

namespace Meta.ORM.Query
{
    internal class QueryProperty
    {
        internal MemberInfo Member { get; set; }

        internal HashSet<QueryProperty> InnerProperties { get; } = new HashSet<QueryProperty>();
    }
}
