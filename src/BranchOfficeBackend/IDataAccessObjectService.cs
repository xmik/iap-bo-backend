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
        void RemoveEmployees(int id);

        List<EmployeeHours> GetAllEmployeeHours(int employeeId);
        EmployeeHours GetOneEmployeeHours(int employeeHoursId);
        void AddEmployeeHours(EmployeeHours employeeHours);
    }
}
