using System;
using System.Collections.Generic;
using System.Linq;

namespace BranchOfficeBackend
{
    /// <summary>
    /// Implementation of interacting with PostgreSQL database
    /// </summary>
    public class PostgresDataAccessObjectService : IDataAccessObjectService
    {
        private readonly BranchOfficeDbContext dbContext;

        public PostgresDataAccessObjectService(BranchOfficeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<Employee> GetAllEmployees()
        {
            return dbContext.Employees.ToList();
        }

        public Employee GetEmployee(int id)
        {
            var employees = dbContext.Employees.ToList();
            var oneEmp = employees.Where(obj => obj.EmployeeId == id);
            return (Employee)(oneEmp.FirstOrDefault());
        }

        public void RemoveEmployees(int id)
        {
            throw new NotImplementedException();
        }

        public List<EmployeeHours> GetAllEmployeeHours(int employeeId)
        {
            var all = dbContext.EmployeeHoursCollection.ToList();
            var selectedByEmployeeId = all.Where(obj => obj.EmployeeId == employeeId);
            return selectedByEmployeeId.ToList();
        }

        public EmployeeHours GetOneEmployeeHours(int employeeHoursId)
        {
            var all = dbContext.EmployeeHoursCollection.ToList();
            var selectedByEmployeeHoursId = all.Where(obj => obj.EmployeeHoursId == employeeHoursId);
            return selectedByEmployeeHoursId.ToList().First();
        }
    }
}
