namespace Meta.ORM.Model
{
    public interface IRuntimeProperty : IProperty
    {
        IModelProperty ModelProperty { get; }
    }
}