using System.Collections.Generic;
using System.Linq;

namespace BranchOfficeBackend
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDataAccessObjectService daoService;
        public EmployeeRepository(IDataAccessObjectService daoService)
        {
            this.daoService = daoService;
        }

        public void AddEmployee(WebEmployee employee)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteEmployee(int employeeId)
        {
            throw new System.NotImplementedException();
        }

        public List<WebEmployee> GetAllEmployees()
        {
            var dbEmployees = daoService.ListEmployees();
            return dbEmployees.Select(e => new WebEmployee(e)).ToList();
        }
    }
}