using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Meta.ORM
{
    public interface IDbProvider
    {
        IDbConnection GetConnection();

        IDbCommand GetCommand();

        IDbDataAdapter GetAdapter();

        IDataReader GetReader();
    }
}
