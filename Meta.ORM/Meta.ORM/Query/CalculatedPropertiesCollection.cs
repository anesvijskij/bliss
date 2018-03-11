using System.Collections.Generic;

namespace Meta.ORM.Query
{
    public class CalculatedPropertiesCollection
    {
        readonly Dictionary<string, ICalculatedProperty> _definitions = new Dictionary<string, ICalculatedProperty>();

        public CalculatedProperty<T> GetProperty<T>(string name) 
        {
            return (CalculatedProperty<T>)_definitions[name];
        }

        public object GetPropertyValue(string name)
        {
            return ((ICalculatedProperty<object>)_definitions[name]).Value;
        }

        public T GetPropertyValue<T>(string name)
        {
            return ((ICalculatedProperty<T>)_definitions[name]).Value;
        }

        public void AddProperty(string name, ICalculatedProperty property) 
        {
            _definitions.Add(name, property);
        }
    }
}