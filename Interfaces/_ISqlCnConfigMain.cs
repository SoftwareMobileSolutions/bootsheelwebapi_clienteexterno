using System.Collections.Generic;
using System.Threading.Tasks;
using bootshellwebapi.Models;
namespace bootshellwebapi.Interfaces
{
    public interface _ISqlCnConfigMain
    {
        Task<IEnumerable<_SqlCnConfigMainModel>> GetCn(string origen);
    }
}
