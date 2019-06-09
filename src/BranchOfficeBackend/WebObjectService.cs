using System.Collections.Generic;
using System.Linq;

namespace BranchOfficeBackend
{
    public class WebObjectService : IWebObjectService
    {
        private readonly IDataAccessObjectService daoService;
        public WebObjectService(IDataAccessObjectService daoService)
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
            var dbEmployees = daoService.GetAllEmployees();
            return dbEmployees.Select(e => new WebEmployee(e)).ToList();
        }

        public WebEmployee GetEmployee(int employeeId)
        {
            var dbEmployee = daoService.GetEmployee(employeeId);
            return new WebEmployee(dbEmployee);
        }

        public List<WebEmployeeHours> GetAllEmployeeHours(int employeeId)
        {
            var coll = daoService.GetAllEmployeeHours(employeeId);
            return coll.Select(e => new WebEmployeeHours(e)).ToList();
        }

        public WebEmployeeHours GetOneEmployeeHours(int employeeHoursId)
        {
            var weh = daoService.GetOneEmployeeHours(employeeHoursId);
            return new WebEmployeeHours(weh);
        }
    }
}
