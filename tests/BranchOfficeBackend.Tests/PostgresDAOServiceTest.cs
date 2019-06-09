using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BranchOfficeBackend.Tests
{
    [Collection("do-not-run-in-parallel")]
    /// <summary>
    /// Tests the DAO layer - connection with Postgres db
    /// </summary>
    public class PostgresDAOServiceTest : IDisposable
    {
        private BranchOfficeDbContext dbContext;

        public PostgresDAOServiceTest()
        {
            dbContext = new BranchOfficeDbContext();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
        }

        public void Dispose()
        {
            if (dbContext != null)
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Dispose();
                dbContext = null;
            }
        }

        [Fact]
        public void GetAllEmployees_ShouldReturnEmptyListWhenDBIsEmpty()
        {
            var dao = new PostgresDataAccessObjectService(dbContext);
            var employees = dao.GetAllEmployees();
            Assert.Empty(employees);
        }

        [Fact]
        public async Task GetAllEmployees_ShouldReturnAllEmployeesWhenSomeInDB()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Dwa", Email = "ola2@gmail.com" });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var employees = dao.GetAllEmployees();
            Assert.Single(employees);
        }

        [Fact]
        public async Task GetEmployee_ShouldReturnSpecifiedEmployee_IfExists()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Dwa", Email = "ola2@gmail.com", EmployeeId = 0 });
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var employee = dao.GetEmployee(4);
            Assert.NotNull(employee);
            Assert.Equal("Ola AAA", employee.Name);
            Assert.Equal(4, employee.EmployeeId);
            Assert.Equal("aaaa@gmail.com", employee.Email);
        }

        [Fact]
        public async Task GetEmployee_ShouldReturnNull_IfNoEmployee()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Dwa", Email = "ola2@gmail.com", EmployeeId = 0 });
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var employee = dao.GetEmployee(55);
            Assert.Null(employee);
        }

        [Fact]
        public void GetAllEmployeeHours_ShouldReturnEmptyListWhenDBIsEmpty()
        {
            var dao = new PostgresDataAccessObjectService(dbContext);
            var coll = dao.GetAllEmployeeHours(0);
            Assert.Empty(coll);
        }

        [Fact]
        public async Task GetAllEmployeeHours_ShouldReturnAllEmployeesHoursWhenSomeInDB()
        {
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 0});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var coll = dao.GetAllEmployeeHours(0);
            Assert.Single(coll);
        }
    }
}
