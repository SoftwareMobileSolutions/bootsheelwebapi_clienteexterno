using System.Collections.Generic;
using System.Threading.Tasks;
using bootshellwebapi.Models;

namespace bootshellwebapi.Interfaces
{
    public interface ICompanykey
    {
        Task<IEnumerable<CompanykeyModel>> ObtenerCompanyId(_ISqlCnConfigMain _ISqlCnConfigMain_, string origen, string companyidkey);
    }
}
