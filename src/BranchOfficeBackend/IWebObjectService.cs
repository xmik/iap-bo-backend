using System.Collections.Generic;

namespace BranchOfficeBackend
{
    /// <summary>
    /// Translates DAL objects onto API Server (Web) objects.
    /// (Because e.g. Employee object in db may be different than
    /// the Employee object returned by API Server).
    /// The translation is kept here in order to obey Single Responsibility Rule.
    /// </summary>
    public interface IWebObjectService
    {
        /// <summary>
        /// Lists all employees.Throws exception on any problem
        /// </summary>
        /// <returns></returns>
        List<WebEmployee> GetAllEmployees();

        WebEmployee GetEmployee(int employeeId);

        List<WebEmployeeHours> GetEmployeeHoursForAnEmployee(int employeeId);
        WebEmployeeHours GetOneEmployeeHours(int employeeHoursId);
        void AddEmployeeHours(WebEmployeeHours employeeHours);
        void DeleteEmployeeHours(int employeeHoursId);
        void EditEmployeeHours(WebEmployeeHours employeeHours);

        List<WebSalary> GetAllSalaries();
        List<WebSalary> GetSalariesForAnEmployee(int employeeId);

    }
}
