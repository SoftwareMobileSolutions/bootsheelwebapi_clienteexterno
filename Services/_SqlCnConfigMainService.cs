using bootshellwebapi.Interfaces;
using bootshellwebapi.Models;
using bootshellwebapi.Data;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bootshellwebapi.Services
{
    public class _SqlCnConfigMainService : _ISqlCnConfigMain
    {
        private readonly SqlCnConfigMain _configuration;
        public _SqlCnConfigMainService(SqlCnConfigMain configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<_SqlCnConfigMainModel>> GetCn(string origen)
        {
            IEnumerable<_SqlCnConfigMainModel> data;
            using (var conn = new SqlConnection(_configuration.Value))
            {
                string query = @"exec bootshellwebapi_getOrigenConsulta @origen";

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                data = await conn.QueryAsync<_SqlCnConfigMainModel>(query, new { origen }, commandType: CommandType.Text);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return data;
        }

    }
}
