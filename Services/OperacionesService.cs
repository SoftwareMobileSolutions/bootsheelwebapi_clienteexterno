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
    public class OperacionesService: IOperaciones
    {
        public async Task<IEnumerable<OperacionesModel>> ObtenerSPFromTblOperacion(_ISqlCnConfigMain _ISqlCnConfigMain_, string origen, string operacionid)
        {
            SqlCn SqlCn = new SqlCn();
            string conexion = await SqlCn.getConexion(_ISqlCnConfigMain_, origen);

            IEnumerable<OperacionesModel> data;
            using (var conn = new SqlConnection(conexion))
            {
                string query = @"exec bootshellwebapi_obteneroperaciones @operacionid";
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                data = await conn.QueryAsync<OperacionesModel>(query, new { operacionid }, commandType: CommandType.Text);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return data;
        }

      
    }
}
