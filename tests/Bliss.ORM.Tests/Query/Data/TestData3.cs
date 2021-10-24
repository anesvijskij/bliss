using System;
using Bliss.ORM.Entities;
using Bliss.ORM.Model;

namespace Bliss.ORM.Tests.Query.Data
{
    public class TestData3 : IEntity
    {
        public string Name { get; set; }

        public int Property1Id { get; set; }

        public TestData1 Property1 { get; set; }

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public object GetPropertyValue(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Name):
                {
                    return Name;
                }
                case nameof(Property1):
                {
                    return Property1;
                }
                case nameof(Property1Id):
                {
                    return Property1Id;
                }
                default:
                    throw new ArgumentException(nameof(propertyName));
            }
        }

        /// <inheritdoc />
        public T GetPropertyValue<T>(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Name) when Name is T tName:
                {
                    return tName;
                }
                case nameof(Property1) when Property1 is T tProperty1:
                {
                    return tProperty1;
                }
                case nameof(Property1Id) when Property1Id is T tProperty1Id:
                {
                    return tProperty1Id;
                }
                default:
                    throw new ArgumentException(nameof(propertyName));
            }
        }

        /// <inheritdoc />
        public object GetPropertyValue(IProperty property)
        {
            return GetPropertyValue(property.Name);
        }

        /// <inheritdoc />
        public T GetPropertyValue<T>(IProperty propertyName)
        {
            return GetPropertyValue<T>(propertyName.Name);
        }

        /// <inheritdoc />
        public T GetPropertyValue<T>(IProperty<T> property)
        {
            return GetPropertyValue<T>(property.Name);
        }
    }
}