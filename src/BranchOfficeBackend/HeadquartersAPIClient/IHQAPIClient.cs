using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BranchOfficeBackend
{
    public interface IHQAPIClient : IDisposable
    {
        Task<HQBranchOffice> GetBranchOffice(int branchOfficeId);
        Task<List<HQBranchOffice>> ListBranchOffices();
        Task<HQEmployee> GetEmployee(int employeeId);
        Task<List<HQEmployee>> ListEmployees(int branchOfficeId);
        Task<List<HQSalary>> ListSalariesForEmployee(int employeeId);
    }
}
