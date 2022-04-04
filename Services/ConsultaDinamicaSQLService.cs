using bootshellwebapi.Interfaces;
using bootshellwebapi.Data;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using bootshellwebapi.Models;

namespace bootshellwebapi.Services
{
    public class ConsultaDinamicaSQLService : IConsultaDinamicaSQL
    {
        public async Task<IEnumerable<dynamic>> consultaDinamicaSQL(/*_ISqlCnConfigMain _ISqlCnConfigMain_,*/ string conexion, string spname, string strspparams, Dictionary<string, object> parametros)
        {
            //SqlCn SqlCn = new SqlCn();
            //string conexion = await SqlCn.getConexion(_ISqlCnConfigMain_, origen);
            IEnumerable<dynamic> data;
            using (var conn = new SqlConnection(conexion))
            {
                string query = @"exec " + spname + " " + strspparams;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                data = await conn.QueryAsync<dynamic>(query, parametros, commandType: CommandType.Text);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return data;
        }

        public async Task<IEnumerable<CustomModel.rpCalificacionMotoristas>> consultaDinamicaSQL_calmotorista(/*_ISqlCnConfigMain _ISqlCnConfigMain_,*/ string conexion, string spname, string strspparams, Dictionary<string, object> parametros)
        {
            //SqlCn SqlCn = new SqlCn();
            //string conexion = await SqlCn.getConexion(_ISqlCnConfigMain_, origen);
            IEnumerable<CustomModel.rpCalificacionMotoristas> data;
            using (var conn = new SqlConnection(conexion))
            {
                string query = @"exec " + spname + " " + strspparams;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                data = await conn.QueryAsync<CustomModel.rpCalificacionMotoristas>(query, parametros, commandType: CommandType.Text);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return data;
        }
    }
}
