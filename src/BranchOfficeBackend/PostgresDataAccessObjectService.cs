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
    }
}
