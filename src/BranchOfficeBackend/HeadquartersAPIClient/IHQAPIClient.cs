using System.Collections.Generic;
using System.Threading.Tasks;

namespace BranchOfficeBackend
{
    public interface IHQAPIClient
    {
        Task<HQBranchOffice> GetBranchOffice(int branchOfficeId);
        Task<List<HQBranchOffice>> ListBranchOffices();
    }
}
