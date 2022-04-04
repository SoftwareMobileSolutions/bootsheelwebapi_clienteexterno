using System.Collections.Generic;
using System.Threading.Tasks;
using bootshellwebapi.Models;

namespace bootshellwebapi.Interfaces
{
    public interface IOperaciones
    {
        Task<IEnumerable<OperacionesModel>> ObtenerSPFromTblOperacion(_ISqlCnConfigMain _ISqlCnConfigMain_, string origen, string operacionid);
    }
}
