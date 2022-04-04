using System.Collections.Generic;
using System.Threading.Tasks;
using bootshellwebapi.Models;
namespace bootshellwebapi.Interfaces
{
    public interface IConsultaDinamicaSQL
    {
        Task<IEnumerable<dynamic>> consultaDinamicaSQL(/*_ISqlCnConfigMain _ISqlCnConfigMain_,*/ string origen, string spname, string strspparams, Dictionary<string, object> parametros);
        Task<IEnumerable<CustomModel.rpCalificacionMotoristas>> consultaDinamicaSQL_calmotorista(/*_ISqlCnConfigMain _ISqlCnConfigMain_,*/ string conexion, string spname, string strspparams, Dictionary<string, object> parametros);
    }
}
