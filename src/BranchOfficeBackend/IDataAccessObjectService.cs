using System.Collections.Generic;

namespace BranchOfficeBackend
{
    /// <summary>
    /// Data access layer, one of the implementations could be interacting with some database
    /// </summary>
    public interface IDataAccessObjectService
    {
        List<Employee> GetAllEmployees();
        Employee GetOneEmployee(int id);
        void AddEmployee(Employee employee, bool keepId=false);
        void DeleteEmployee(int employeeId);
        void EditEmployee(Employee emp);

        int GetEmployeeIdByMail(string mail);

        List<EmployeeHours> GetEmployeeHoursForAnEmployee(int employeeId);
        List<EmployeeHours> GetAllEmployeeHours();
        EmployeeHours GetOneEmployeeHours(int employeeHoursId);
        void AddEmployeeHours(EmployeeHours employeeHours, bool keepId=false);
        void DeleteEmployeeHours(int employeeHoursId);
        void EditEmployeeHours(EmployeeHours employeeHours);

        List<Salary> GetAllSalaries();
        List<Salary> GetSalariesForAnEmployee(int employeeId);
        Salary GetOneSalary(int id);
        void AddSalary(Salary salary, bool keepId=false);
        void DeleteSalary(int salaryId);

        void InformOnDBContents();
    }
}
