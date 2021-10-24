using System.Collections.Generic;
using System.Reflection;

namespace Bliss.ORM.Query
{
    internal class QueryProperty
    {
        public QueryProperty(MemberInfo member)
        {
            Member = member;
        }

        internal MemberInfo Member { get; set; }

        internal HashSet<QueryProperty> InnerProperties { get; } = new HashSet<QueryProperty>();
    }
}
