using System.Collections.Generic;
using System.Threading.Tasks;
using bootshellwebapi.Models;

namespace bootshellwebapi.Interfaces
{
    public interface ICustom
    {
        Task<IEnumerable<CompanykeyModel>> ObtenerCompanyId(string origen, string companyidkey);
    }
}
