using Meta.ORM.Entities;
using Meta.ORM.Sql;
using Meta.ORM.Tests.Query;
using Xunit;

namespace Meta.ORM.Tests.Sql
{
    using Meta.ORM.Tests.Query.Data;

    public class SqlBuilderTests
    {
        private readonly SqlBuilder _sut;

        public SqlBuilderTests()
        {
            _sut = new SqlBuilder();
        }

        [Fact]
        public void SelectTest()
        {
            _sut.Select<TestData1>().Column(data1 => data1.Name).Column(data1 => data1.Property2);
        }
    }
}
