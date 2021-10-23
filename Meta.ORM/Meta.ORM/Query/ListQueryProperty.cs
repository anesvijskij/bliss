using System.Reflection;

namespace Meta.ORM.Query
{
    internal class ListQueryProperty<T> : QueryProperty
    {
        public ListQueryProperty(MemberInfo member, Query<T> listQuery) : base(member)
        {
            ListQuery = listQuery;
        }

        public Query<T> ListQuery { get; set; } 
    }
}