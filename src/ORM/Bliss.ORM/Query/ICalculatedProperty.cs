namespace Bliss.ORM.Query
{
    public interface ICalculatedProperty<out T> : ICalculatedProperty
    {
        T Value { get; }
    }

    public interface ICalculatedProperty
    {
        string Name { get; set; }
    }
}