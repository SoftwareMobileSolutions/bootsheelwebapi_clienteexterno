using System.Threading.Tasks;
using bootshellwebapi.Interfaces;
using System.Linq;
namespace bootshellwebapi.Data
{
    public class SqlCn
    {
        public async Task<string> getConexion(_ISqlCnConfigMain _ISqlCnConfigMain, string origen)
        {
            origen = origen.ToUpper();
            var cnData = await _ISqlCnConfigMain.GetCn(origen);
            string conexion = "";
            if (cnData != null)
            {
                var cn = cnData.First();
                conexion = "data source="+ cn.source +";initial catalog=" + cn.catalog +";user id="+ cn.user  + ";password="+ cn.pass + "; Trusted_Connection=false; MultipleActiveResultSets=true;";
            }
            return conexion;
        }
    }
}
