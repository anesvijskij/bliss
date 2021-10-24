using Bliss.ORM.Model;
using Bliss.ORM.Query;
using Bliss.ORM.Sql;

namespace Bliss.ORM.Entities
{
    public class GenericEntitiesRepository<T> : IGenericEntitiesRepository<T> where T : class, IEntity, new()
    {
        private readonly IModel _model;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISqlBuilder _sqlBuilder;
        private readonly IDbProvider _dbProvider;

        public GenericEntitiesRepository(IModel model, IUnitOfWork unitOfWork, ISqlBuilder sqlBuilder,
            IDbProvider dbProvider)
        {
            _model = model;
            _unitOfWork = unitOfWork;
            _sqlBuilder = sqlBuilder;
            _dbProvider = dbProvider;
        }

        public Query<T> Query()
        {
            return new Query<T>(_model, _unitOfWork, _sqlBuilder, _dbProvider);
        }

        public T Create()
        {
            return new T();
        }
    }
}
