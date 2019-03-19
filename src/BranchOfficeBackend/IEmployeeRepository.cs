using System.Collections.Generic;

namespace BranchOfficeBackend
{
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Throws exception on any problem
        /// </summary>
        /// <returns></returns>
        List<WebEmployee> GetAllEmployees();

        /// <summary>
        /// Adds employee if the employee object is valid, throws exception otherwise.
        /// </summary>
        /// <param name="employee"></param>
        void AddEmployee(WebEmployee employee);

        /// <summary>
        /// Deletes employee if it exists, throws exception otherwise.
        /// </summary>
        /// <param name="employeeId"></param>
        void DeleteEmployee(int employeeId);

    }
}