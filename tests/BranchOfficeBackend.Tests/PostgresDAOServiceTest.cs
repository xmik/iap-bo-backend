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
        public void ShouldReturnEmptyListWhenDBIsEmpty()
        {
            var dao = new PostgresDataAccessObjectService(dbContext);
            var employees = dao.GetAllEmployees();
            Assert.Empty(employees);
        }

        [Fact]
        public async Task ShouldReturnAllEmployeesWhenSomeInDB()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Dwa", Email = "ola2@gmail.com" });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var employees = dao.GetAllEmployees();
            Assert.Single(employees);
        }
    }
}
