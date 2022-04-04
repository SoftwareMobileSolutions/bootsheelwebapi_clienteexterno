using bootshellwebapi.Interfaces;
using bootshellwebapi.Models;
using bootshellwebapi.Data;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace bootshellwebapi.Services
{
    public class CompanykeyService : ICompanykey
    {
       

        public async Task<IEnumerable<CompanykeyModel>> ObtenerCompanyId(_ISqlCnConfigMain _ISqlCnConfigMain_, string origen, string companyidkey)
        {
            SqlCn SqlCn = new SqlCn();
            string conexion = await SqlCn.getConexion(_ISqlCnConfigMain_, origen);

            IEnumerable<CompanykeyModel> data;
            using (var conn = new SqlConnection(conexion))
            {
                string query = @"exec bootshellwebapi_obtenercompanyidByKey @companyidkey";
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                data = await conn.QueryAsync<CompanykeyModel>(query, new { companyidkey }, commandType: CommandType.Text);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return data;
        }
    }
}
