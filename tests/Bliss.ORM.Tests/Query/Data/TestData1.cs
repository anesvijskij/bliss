using System;
using Bliss.ORM.Entities;
using System.Collections.Generic;
using Bliss.ORM.Model;

namespace Bliss.ORM.Tests.Query.Data
{
    public class TestData1 : IEntity
    {
        public string Name { get; set; }

        public int Property2Id { get; set; }

        public TestData2 Property2 { get; set; }

        public int RecursionPropertyId { get; set; }

        public TestData1 RecursionProperty { get; set; }

        public IEnumerable<TestData4> CollectionProperty { get; set; }

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
                case nameof(Property2):
                {
                    return Property2;
                }
                case nameof(Property2Id):
                {
                    return Property2Id;
                }
                case nameof(RecursionProperty):
                {
                    return RecursionProperty;
                }
                case nameof(RecursionPropertyId):
                {
                    return RecursionPropertyId;
                }
                case nameof(CollectionProperty):
                {
                    return CollectionProperty;
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
                case nameof(Property2) when Property2 is T tProperty2:
                {
                    return tProperty2;
                }
                case nameof(Property2Id) when Property2Id is T tProperty2Id:
                {
                    return tProperty2Id;
                }
                case nameof(RecursionProperty) when RecursionProperty is T tRecursionProperty:
                {
                    return tRecursionProperty;
                }
                case nameof(RecursionPropertyId) when RecursionPropertyId is T tRecursionPropertyId:
                {
                    return tRecursionPropertyId;
                }
                case nameof(CollectionProperty) when CollectionProperty is T tCollectionProperty:
                {
                    return tCollectionProperty;
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