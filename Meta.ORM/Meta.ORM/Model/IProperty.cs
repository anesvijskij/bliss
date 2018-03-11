namespace Meta.ORM.Model
{
    public interface IProperty
    {
        IModel GetModel();

        string Name { get; }

        /// <summary>
        /// Определяет, является ли свойство унаследованным или
        /// объявленном непосредственно в том типе, которому оно принадлежит.
        /// </summary>
        bool IsInherited { get; }

        /// <summary>
        /// Тип свойства.
        /// </summary>
        /// <remarks> Для ссылочных типов - тип сущности, на которую ссылается свойство.</remarks>
        IEntityType PropertyType { get; set; }

        /// <summary>
        /// Тип, в котором объявлено данное свойство.
        /// </summary>
        IEntityType DeclaringType { get; }

        /// <summary>
        /// Тип, который был использован для получения информации о свойстве.
        /// </summary>
        IEntityType ReflectedType { get; }
    }

    public interface IProperty<T> : IProperty
    {
    }
}
