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

        public List<WebEmployee> GetAllEmployees()
        {
            var dbEmployees = daoService.GetAllEmployees();
            if (dbEmployees == null)
                return null;
            return dbEmployees.Select(e => new WebEmployee(e)).ToList();
        }

        public WebEmployee GetEmployee(int employeeId)
        {
            var dbEmployee = daoService.GetOneEmployee(employeeId);
            if (dbEmployee == null)
                return null;
            return new WebEmployee(dbEmployee);
        }

        public List<WebEmployeeHours> GetEmployeeHoursForAnEmployee(int employeeId)
        {
            var coll = daoService.GetEmployeeHoursForAnEmployee(employeeId);
            if (coll == null)
                return null;
            return coll.Select(e => new WebEmployeeHours(e)).ToList();
        }

        public WebEmployeeHours GetOneEmployeeHours(int employeeHoursId)
        {
            var weh = daoService.GetOneEmployeeHours(employeeHoursId);
            if (weh == null)
                return null;
            return new WebEmployeeHours(weh);
        }
        public void AddEmployee(WebEmployee employee)
        {
            var eh = new Employee(employee);
            daoService.AddEmployee(eh);
        }
        public void AddEmployeeHours(WebEmployeeHours employeeHours)
        {
            var eh = new EmployeeHours(employeeHours);
            daoService.AddEmployeeHours(eh);
        }

        public void DeleteEmployeeHours(int employeeHoursId)
        {
            daoService.DeleteEmployeeHours(employeeHoursId);
        }

        public void EditEmployeeHours(WebEmployeeHours employeeHours)
        {
            var eh = new EmployeeHours(employeeHours);
            daoService.EditEmployeeHours(eh);
        }

        public List<WebSalary> GetAllSalaries()
        {
            var coll = daoService.GetAllSalaries();
            if (coll == null)
                return null;
            return coll.Select(e => new WebSalary(e)).ToList();
        }
        public List<WebSalary> GetSalariesForAnEmployee(int employeeId)
        {            
            var coll = daoService.GetSalariesForAnEmployee(employeeId);
            if (coll == null)
                return null;
            return coll.Select(e => new WebSalary(e)).ToList();
        }
    }
}
