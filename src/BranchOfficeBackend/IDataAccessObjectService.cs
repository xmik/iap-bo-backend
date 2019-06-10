using System.Collections.Generic;

namespace BranchOfficeBackend
{
    /// <summary>
    /// Data access layer, one of the implementations could be interacting with some database
    /// </summary>
    public interface IDataAccessObjectService
    {
        List<Employee> GetAllEmployees();
        Employee GetEmployee(int id);
        void AddEmployee(Employee employee, bool keepId=false);
        void DeleteEmployee(int employeeId);
        void EditEmployee(Employee emp);

        List<EmployeeHours> GetAllEmployeeHours(int employeeId);
        EmployeeHours GetOneEmployeeHours(int employeeHoursId);
        void AddEmployeeHours(EmployeeHours employeeHours, bool keepId=false);
        void DeleteEmployeeHours(int employeeHoursId);
        void EditEmployeeHours(EmployeeHours employeeHours);

        void InformOnDBContents();
    }
}
