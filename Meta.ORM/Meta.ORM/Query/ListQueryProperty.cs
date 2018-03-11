namespace Meta.ORM.Query
{
    internal class ListQueryProperty<T> : QueryProperty
    {
        public Query<T> ListQuery { get; set; } 
    }
}