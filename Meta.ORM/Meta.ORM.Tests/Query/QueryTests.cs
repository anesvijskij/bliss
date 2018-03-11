using System.Collections.Generic;
using Meta.ORM.Entities;
using Meta.ORM.Expressions;
using Meta.ORM.Model;
using Meta.ORM.Sql;
using Moq;
using Xunit;
using Meta.ORM.Tests.Query.Data;

namespace Meta.ORM.Tests.Query
{
    public class QueryTests
    {
        [Fact]
        public void Select_TypedQuery_Optimized()
        {
            var context = new UnitOfWork(new Mock<IModel>().Object, new Mock<ISqlBuilder>().Object, new Mock<IDbProvider>().Object);

            // New Query
            context
                .GetRepository<TestData1>()
                .Query()

                // Recursion 
                // TODO: can we set recursion after JOIN and SELECT?
                .RecursionStep((data1, data1Step) => data1Step.RecursionProperty == data1)

                // Join Catalog and Projection
                // TODO: we can set additional properties for left join query
                .Join(
                    context.GetRepository<TestData3>().Query(), 
                    (data1, data2) => data1 == data2.Property1,
                    (data1, data2) => new
                                      {
                                          data1.Name,

                                          // Sub property of the reference property
                                          InnerProperty = data1.Property2.Name,

                                          // Calculated property definition
                                          CalculationProperty1 = data1.Name.Length,
                                          Catalog = data2,

                                          // Collection subQuery
                                          // TODO: при таком подходе не понятно, как задавать какие-то параметры такого запроса
                                          Collection = data1.CollectionProperty
                                      })

                // Condition (for first part of the recursion query) with using of calculated property
                .Where(data1 => data1.Name == "1" && data1.CalculationProperty1 == 5)

                // Order
                .OrderBy(data1 => data1.Name)
                .OrderByDescending(data1 => data1.InnerProperty)
                .OrderBy(data1 => data1.CalculationProperty1)

                // Paging
                .Offset(0, 10)

                // Init query
                .ToList();
        }

        [Fact]
        public void Select_TypedQuery_UnOptimized()
        {
            var context = new UnitOfWork(new Mock<IModel>().Object, new Mock<ISqlBuilder>().Object, new Mock<IDbProvider>().Object);

            // New Query
            context
                .GetRepository<TestData1>()
                .Query()

                // Sub property of the reference property
                .Include(data1 => data1.Property2.Name)

                // Collection subQuery
                .IncludeList(
                    data1 => data1.CollectionProperty, 
                    subQuery => subQuery.Only().ForseRefresh().Distinct().IncludeAllReferences().IncludeAllListProperties())

                // Condition (for first part of the recursion query)
                .Where(data1 => data1.Name == "1")

                // Condition (for first part of the recursion query), using of calculated property
                .Where((data1, collection) => data1.Name.Length == 5)

                // Order
                .OrderBy(data1 => data1.Name)
                .OrderByDescending(data1 => data1.Property2.Name)
                .OrderBy(data1 => data1.Name.Length)

                // Paging
                .Offset(0, 10)

                // Recursion 
                .RecursionStep((data1, data1Step) => data1Step.RecursionProperty == data1)

                // Join Catalog
                .Join(
                    context.GetRepository<TestData3>().Query(), 
                    (data1, data2) => data1 == data2.Property1,
                    (data1, data2) => new
                                      {
                                          data1.Name,

                                          // Sub property of the reference property
                                          InnerProperty = data1.Property2.Name,
                                          Catalog = data2
                                      })

                // Projection
                .Select((data1, columns) => new
                                            {
                                                data1.Name,
                                                data1.InnerProperty,

                                                // Calculated property definition
                                                CalculationProperty1 = data1.Name.Length,
                                                data1.Catalog
                                            })

                // Init query
                .ToList();
        }

        [Fact]
        public void Select_UnTypedQuery_WithStringNamedProperties()
        {
            var context = new UnitOfWork(new Mock<IModel>().Object, new Mock<ISqlBuilder>().Object, new Mock<IDbProvider>().Object);

            // New Query
            context
                .GetRepository(typeof(TestData1))
                .Query()

                // Sub property of the reference property
                .Include(entity => entity.GetPropertyValue("Property2.Name"))

                // Calculated property definition
                .IncludeCalculation(
                    entity => entity.GetPropertyValue<string>("Name").Length,
                    "CalculationProperty1")

                // Collection subQuery
                .IncludeList(
                    entity => entity.GetPropertyValue<IEnumerable<TestData4>>("CollectionProperty"),
                    subQuery => subQuery.Only().ForseRefresh().Distinct().IncludeAllReferences().IncludeAllListProperties())

                // Condition (for first part of the recursion query)
                .Where(entity => Equals(entity.GetPropertyValue("Name"), "1"))

                // Condition (for first part of the recursion query), using of calculated property
                .Where((entity, collection) => collection.GetPropertyValue<int>("CalculationProperty1") == 5)

                // Order
                .OrderBy(entity => entity.GetPropertyValue("Name"))
                .OrderByDescending(entity => entity.GetPropertyValue("Property2.Name"))
                .OrderBy((entity, collection) => collection.GetPropertyValue<int>("CalculationProperty1"))

                // Paging
                .Offset(0, 10)

                // Recursion 
                .RecursionStep((entity, entityStep) =>
                                   entity == entityStep.GetPropertyValue("RecursionProperty"))

                // Join Catalog
                .Join(
                    context.GetRepository(typeof(TestData3)).Query(),
                    (entity1, entity2) => entity1 == entity2.GetPropertyValue("Property1"),
                    (entity1, entity2) => new Dictionary<string, object>
                                          {
                                              { "Property", entity1.GetPropertyValue("Name") },
                                              { "InnerProperty", entity1.GetPropertyValue("Property2.Name") },
                                              { "Catalog", entity2 }
                                          })

                // Projection
                .Select((entity, columns) => new Dictionary<string, object>(entity)
                                             {
                                                 { "Property", entity["Property"] },
                                                 { "InnerProperty", entity["InnerProperty"] },
                                                 {
                                                     "CalculationProperty1",
                                                     columns.GetPropertyValue<int>("CalculationProperty1")
                                                 }
                                             })

                // Init query
                .ToList();
        }

        [Fact]
        public void Select_UnTypedQuery_WithIProperty()
        {
            var context = new UnitOfWork(new Mock<IModel>().Object, new Mock<ISqlBuilder>().Object, new Mock<IDbProvider>().Object);

            var mockPropertyName = new Mock<IRuntimeProperty>();
            mockPropertyName.SetupGet(property => property.Name).Returns("Name");
            var propertyName = mockPropertyName.Object; //typeof(TestData1), 

            var mockPropertyPath = new Mock<IPropertyPath>();
            mockPropertyPath.SetupGet(property => property.Name).Returns("Property2.Name");
            var propertyPath = mockPropertyPath.Object; //typeof(TestData1)

            var mockRecursionProperty = new Mock<IRuntimeProperty>();
            mockRecursionProperty.SetupGet(property => property.Name).Returns("RecursionProperty");
            var recursionProperty = mockRecursionProperty.Object; //typeof(TestData1), "RecursionProperty"

            var mockProperty1 = new Mock<IRuntimeProperty>();
            mockProperty1.SetupGet(property => property.Name).Returns("Property1");
            var property1 = mockProperty1.Object; //typeof(TestData3)

            var mockCollectionProperty = new Mock<IRuntimeProperty>();
            mockCollectionProperty.SetupGet(property => property.Name).Returns("CollectionProperty");
            var collectionProperty = mockCollectionProperty.Object; //typeof(TestData3)

            // New Query
            context
                .GetRepository(typeof(TestData1))
                .Query()

                // Sub property of the reference property
                .Include(entity => entity.GetPropertyValue(propertyPath))

                // Calculated property definition
                .IncludeCalculation(entity => entity.GetPropertyValue<string>(propertyName).Length, "CalculationProperty1")

                // Collection subQuery
                .IncludeList(
                    entity => entity.GetPropertyValue<IEnumerable<TestData4>>(collectionProperty),
                    subQuery => subQuery.Only().ForseRefresh().Distinct().IncludeAllReferences().IncludeAllListProperties())

                // Condition (for first part of the recursion query)
                .Where(entity => Equals(entity.GetPropertyValue(propertyName), "1"))

                // Condition (for first part of the recursion query), using of calculated property
                .Where((entity, collection) => collection.GetPropertyValue<int>("CalculationProperty1") == 5)

                // Order
                .OrderBy(entity => entity.GetPropertyValue(propertyName))
                .OrderByDescending(entity => entity.GetPropertyValue(propertyPath))
                .OrderBy((entity, collection) => collection.GetPropertyValue<int>("CalculationProperty1"))

                // Paging
                .Offset(0, 10)

                // Recursion 
                .RecursionStep((entity, entityStep) => entity == entityStep.GetPropertyValue(recursionProperty))

                // Join Catalog
                .Join(
                    context.GetRepository(typeof(TestData3)).Query(),
                    (entity1, entity2) => entity1 == entity2.GetPropertyValue(property1),
                    (entity1, entity2) => new Dictionary<string, object>
                                          {
                                              { "Property", entity1.GetPropertyValue(propertyName) },
                                              { "InnerProperty", entity1.GetPropertyValue(propertyPath) },
                                              { "Catalog", entity2 }
                                          })

                // Projection
                .Select((entity, columns) => new Dictionary<string, object>(entity)
                                             {
                                                 { "Property", entity["Property"] },
                                                 { "InnerProperty", entity["InnerProperty"] },
                                                 {
                                                     "CalculationProperty1",
                                                     columns.GetPropertyValue<int>("CalculationProperty1")
                                                 }
                                             })

                // Init query
                .ToList();
        }

        [Fact]
        public void Select_UnTypedQuery_WithITypedProperty()
        {
            var context = new UnitOfWork(new Mock<IModel>().Object, new Mock<ISqlBuilder>().Object, new Mock<IDbProvider>().Object);

            var mockPropertyName = new Mock<IProperty<string>>();
            mockPropertyName.SetupGet(property => property.Name).Returns("Name");
            var propertyName = mockPropertyName.Object; //typeof(TestData1), 

            var mockPropertyPath = new Mock<IProperty<string>>();
            mockPropertyPath.SetupGet(property => property.Name).Returns("Property2.Name");
            var propertyPath = mockPropertyPath.Object; //typeof(TestData1)

            var mockRecursionProperty = new Mock<IProperty<TestData1>>();
            mockRecursionProperty.SetupGet(property => property.Name).Returns("RecursionProperty");
            var recursionProperty = mockRecursionProperty.Object; //typeof(TestData1), "RecursionProperty"

            var mockProperty1 = new Mock<IProperty<TestData1>>();
            mockProperty1.SetupGet(property => property.Name).Returns("Property1");
            var property1 = mockProperty1.Object; //typeof(TestData3)

            var mockCollectionProperty = new Mock<IProperty<IEnumerable<TestData4>>>();
            mockCollectionProperty.SetupGet(property => property.Name).Returns("CollectionProperty");
            var collectionProperty = mockCollectionProperty.Object; //typeof(TestData3)

            // New Query
            context
                .GetRepository(typeof(TestData1))
                .Query()

                // Sub property of the reference property
                .Include(entity => entity.GetPropertyValue(propertyPath))

                // Calculated property definition
                .IncludeCalculation(entity => entity.GetPropertyValue(propertyName).Length, "CalculationProperty1")

                // Collection subQuery
                .IncludeList(
                    entity => entity.GetPropertyValue(collectionProperty),
                    subQuery => subQuery.Only().ForseRefresh().Distinct().IncludeAllReferences().IncludeAllListProperties())

                // Condition (for first part of the recursion query)
                .Where(entity => Equals(entity.GetPropertyValue(propertyName), "1"))

                // Condition (for first part of the recursion query), using of calculated property
                .Where((entity, collection) => collection.GetPropertyValue<int>("CalculationProperty1") == 5)

                // Order
                .OrderBy(entity => entity.GetPropertyValue(propertyName))
                .OrderByDescending(entity => entity.GetPropertyValue(propertyPath))
                .OrderBy((entity, collection) => collection.GetPropertyValue<int>("CalculationProperty1"))

                // Paging
                .Offset(0, 10)

                // Recursion 
                .RecursionStep((entity, entityStep) => entity == entity.GetPropertyValue(recursionProperty))

                // Join Catalog
                .Join(
                    context.GetRepository(typeof(TestData3)).Query(),
                    (entity1, entity2) => entity1 == entity2.GetPropertyValue(property1),
                    (entity1, entity2) => new Dictionary<string, object>
                                          {
                                              { "Property", entity1.GetPropertyValue(propertyName) },
                                              { "InnerProperty", entity1.GetPropertyValue(propertyPath) },
                                              { "Catalog", entity2 }
                                          })

                // Projection
                .Select((entity, columns) => new Dictionary<string, object>
                                             {
                                                 { "Property", entity["Property"] },
                                                 { "InnerProperty", entity["InnerProperty"] },
                                                 {
                                                     "CalculationProperty1",
                                                     columns.GetPropertyValue<int>("CalculationProperty1")
                                                 }
                                             })

                // Init query
                .ToList();
        }

        [Fact]
        public void Select_UnTypedQuery_WithIPropertyAndMetaExpression()
        {
            var context = new UnitOfWork(new Mock<IModel>().Object, new Mock<ISqlBuilder>().Object, new Mock<IDbProvider>().Object);

            var mockPropertyName = new Mock<IRuntimeProperty>();
            mockPropertyName.SetupGet(property => property.Name).Returns("Name");
            var propertyName = mockPropertyName.Object; //typeof(TestData1), 

            var mockPropertyPath = new Mock<IPropertyPath>();
            mockPropertyPath.SetupGet(property => property.Name).Returns("Property2.Name");
            var propertyPath = mockPropertyPath.Object; //typeof(TestData1)

            var mockRecursionProperty = new Mock<IRuntimeProperty>();
            mockRecursionProperty.SetupGet(property => property.Name).Returns("RecursionProperty");
            var recursionProperty = mockRecursionProperty.Object; //typeof(TestData1), "RecursionProperty"

            var mockProperty1 = new Mock<IRuntimeProperty>();
            mockProperty1.SetupGet(property => property.Name).Returns("Property1");
            var property1 = mockProperty1.Object; //typeof(TestData3)

            var mockCollectionProperty = new Mock<IRuntimeProperty>();
            mockCollectionProperty.SetupGet(property => property.Name).Returns("CollectionProperty");
            var collectionProperty = mockCollectionProperty.Object; //typeof(TestData3)

            // New Query
            context
                .GetRepository(typeof(TestData1))
                .Query()

                // Sub property of the reference property
                .Include(entity => entity.GetPropertyValue(propertyPath))

                // Calculated property definition
                .IncludeCalculation(MetaExpression.Property(propertyName).StringLength(), "CalculationProperty1")

                // Collection subQuery
                .IncludeList(
                    entity => entity.GetPropertyValue<IEnumerable<IEntity>>(collectionProperty),
                    subQuery => subQuery.Only().ForseRefresh().Distinct().IncludeAllReferences().IncludeAllListProperties())

                // Condition (for first part of the recursion query)
                .Where(MetaExpression.Property(propertyName).Equal(MetaExpression.Constant("1")))

                // Condition (for first part of the recursion query), using of calculated property
                .Where(MetaExpression.Property("CalculationProperty1").Equal(MetaExpression.Constant(5)))

                // Order
                .OrderBy(entity => entity.GetPropertyValue(propertyName))
                .OrderByDescending(entity => entity.GetPropertyValue(propertyPath))
                .OrderBy((entity, collection) => collection.GetPropertyValue("CalculationProperty1"))

                // Paging
                .Offset(0, 10)

                // Recursion 
                .RecursionStep(MetaExpression.Property(recursionProperty).Equal(MetaExpression.Container()))

                // Join Catalog
                .Join(
                    context.GetRepository(typeof(TestData3)).Query(),
                    MetaExpression.Property(property1).Equal(MetaExpression.Container()),
                    (entity1, entity2) => new Dictionary<string, object>
                                          {
                                              { "Property", entity1.GetPropertyValue(propertyName) },
                                              { "InnerProperty", entity1.GetPropertyValue(propertyPath) },
                                              { "Catalog", entity2 }
                                          })

                // Projection
                .Select((entity, columns) => new Dictionary<string, object>
                                             {
                                                 { "Property", entity["Property"] },
                                                 { "InnerProperty", entity["InnerProperty"] },
                                                 {
                                                     "CalculationProperty1",
                                                     columns.GetPropertyValue("CalculationProperty1")
                                                 }
                                             })

                // Init query
                .ToList();
        }

        [Fact]
        public void Select_UnTypedQuery_WithITypedPropertyAndMetaExpression()
        {
            var context = new UnitOfWork(new Mock<IModel>().Object, new Mock<ISqlBuilder>().Object, new Mock<IDbProvider>().Object);

            var mockPropertyName = new Mock<IProperty<string>>();
            mockPropertyName.SetupGet(property => property.Name).Returns("Name");
            var propertyName = mockPropertyName.Object; //typeof(TestData1), 

            var mockPropertyPath = new Mock<IProperty<string>>();
            mockPropertyPath.SetupGet(property => property.Name).Returns("Property2.Name");
            var propertyPath = mockPropertyPath.Object; //typeof(TestData1)

            var mockRecursionProperty = new Mock<IProperty<TestData1>>();
            mockRecursionProperty.SetupGet(property => property.Name).Returns("RecursionProperty");
            var recursionProperty = mockRecursionProperty.Object; //typeof(TestData1), "RecursionProperty"

            var mockProperty1 = new Mock<IProperty<TestData1>>();
            mockProperty1.SetupGet(property => property.Name).Returns("Property1");
            var property1 = mockProperty1.Object; //typeof(TestData3)

            var mockCollectionProperty = new Mock<IProperty<IEnumerable<TestData4>>>();
            mockCollectionProperty.SetupGet(property => property.Name).Returns("CollectionProperty");
            var collectionProperty = mockCollectionProperty.Object; //typeof(TestData3)

            // New Query
            context
                .GetRepository(typeof(TestData1))
                .Query()

                // Sub property of the reference property
                .Include(entity => entity.GetPropertyValue(propertyPath))

                // Calculated property definition
                .IncludeCalculation(MetaExpression.Property(propertyName).StringLength(), "CalculationProperty1")

                // Collection subQuery
                .IncludeList(
                    entity => entity.GetPropertyValue(collectionProperty),
                    subQuery => subQuery.Only().ForseRefresh().Distinct().IncludeAllReferences().IncludeAllListProperties())

                // Condition (for first part of the recursion query)
                .Where(MetaExpression.Property(propertyName).Equal(MetaExpression.Constant("1")))

                // Condition (for first part of the recursion query), using of calculated property
                .Where(MetaExpression.Property("CalculationProperty1").Equal(MetaExpression.Constant(5)))

                // Order
                .OrderBy(entity => entity.GetPropertyValue(propertyName))
                .OrderByDescending(entity => entity.GetPropertyValue(propertyPath))
                .OrderBy((entity, collection) => collection.GetPropertyValue("CalculationProperty1"))

                // Paging
                .Offset(0, 10)

                // Recursion 
                .RecursionStep(MetaExpression.Property(recursionProperty).Equal(MetaExpression.Container()))

                // Join Catalog
                .Join(
                    context.GetRepository(typeof(TestData3)).Query(),
                    MetaExpression.Property(property1).Equal(MetaExpression.Container()),
                    (entity1, entity2) => new Dictionary<string, object>
                                          {
                                              { "Property", entity1.GetPropertyValue(propertyName) },
                                              { "InnerProperty", entity1.GetPropertyValue(propertyPath) },
                                              { "Catalog", entity2 }
                                          })

                // Projection
                .Select((entity, columns) => new Dictionary<string, object>
                                             {
                                                 { "Property", entity["Property"] },
                                                 { "InnerProperty", entity["InnerProperty"] },
                                                 {
                                                     "CalculationProperty1",
                                                     columns.GetPropertyValue("CalculationProperty1")
                                                 }
                                             })

                // Init query
                .ToList();
        }

        [Fact]
        public void Select_UnTypedQuery_WithIPropertyAndMetaExpressionParse()
        {
            var context = new UnitOfWork(new Mock<IModel>().Object, new Mock<ISqlBuilder>().Object, new Mock<IDbProvider>().Object);

            var mockPropertyName = new Mock<IRuntimeProperty>();
            mockPropertyName.SetupGet(property => property.Name).Returns("Name");
            var propertyName = mockPropertyName.Object; //typeof(TestData1), 

            var mockPropertyPath = new Mock<IPropertyPath>();
            mockPropertyPath.SetupGet(property => property.Name).Returns("Property2.Name");
            var propertyPath = mockPropertyPath.Object; //typeof(TestData1)

            var mockCollectionProperty = new Mock<IRuntimeProperty>();
            mockCollectionProperty.SetupGet(property => property.Name).Returns("CollectionProperty");
            var collectionProperty = mockCollectionProperty.Object; //typeof(TestData3)

            ////var recursionProperty = new Mock<IRuntimeProperty>(typeof(TestData1), "RecursionProperty").Object;
            ////var property1 = new Mock<IRuntimeProperty>(typeof(TestData3), "Property1").Object;

            // New Query
            context
                .GetRepository(typeof(TestData1))
                .Query()

                // Sub property of the reference property
                .Include(entity => entity.GetPropertyValue(propertyPath))

                // Calculated property definition
                .IncludeCalculation(MetaExpression.Parse("StringLength([Name])"), "CalculationProperty1")

                // Collection subQuery
                .IncludeList(
                    entity => entity.GetPropertyValue<IEnumerable<IEntity>>(collectionProperty),
                    subQuery => subQuery.Only().ForseRefresh().Distinct().IncludeAllReferences().IncludeAllListProperties())

                // Condition (for first part of the recursion query)
                .Where(MetaExpression.Parse<bool>("[Name] == '1'"))

                // Condition (for first part of the recursion query), using of calculated property
                .Where(MetaExpression.Parse<bool>("[CalculationProperty1] == 5"))

                // Order
                .OrderBy(entity => entity.GetPropertyValue(propertyName))
                .OrderByDescending(entity => entity.GetPropertyValue(propertyPath))
                .OrderBy((entity, collection) => collection.GetPropertyValue("CalculationProperty1"))

                // Paging
                .Offset(0, 10)

                // Recursion 
                .RecursionStep(MetaExpression.Parse<bool>("[RecursionProperty] == Container()"))

                // Join Catalog
                .Join(
                    context.GetRepository(typeof(TestData3)).Query(),
                    MetaExpression.Parse<bool>("[Property1] == Container()"),
                    (entity1, entity2) => new Dictionary<string, object>
                                          {
                                              { "Property", entity1.GetPropertyValue(propertyName) },
                                              { "InnerProperty", entity1.GetPropertyValue(propertyPath) },
                                              { "Catalog", entity2 }
                                          })

                // Projection
                .Select((entity, columns) => new Dictionary<string, object>
                                             {
                                                 { "Property", entity["Property"] },
                                                 { "InnerProperty", entity["InnerProperty"] },
                                                 {
                                                     "CalculationProperty1",
                                                     columns.GetPropertyValue("CalculationProperty1")
                                                 }
                                             })

                // Init query
                .ToList();
        }
    }
}