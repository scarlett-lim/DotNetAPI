using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI
{
    class DataContextDapper
    {
        private readonly IConfiguration _config;
        private readonly IDbConnection _dbConnection;

        public DataContextDapper(IConfiguration config)
        {
            //Initialize config to read from appsettings.json
            _config = config;

            //Build connection to sql
            _dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        // Return multiple line of records from db
        public IEnumerable<T> LoadData<T>(string sql)
        {
            return _dbConnection.Query<T>(sql);
        }

        // Return singlerecord from db
        public T LoadDataSingle<T>(string sql)
        {
            return _dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSql(string sql)
        {
            return _dbConnection.Execute(sql) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql)
        {
            return _dbConnection.Execute(sql);
        }
    }
}