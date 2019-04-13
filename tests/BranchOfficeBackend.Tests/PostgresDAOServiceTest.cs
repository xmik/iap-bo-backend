using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BranchOfficeBackend.Tests
{
    [Collection("do-not-run-in-parallel")]
    public class PostgresDAOServiceTest : IDisposable
    {
        private BranchOfficeDbContext dbContext;

        public PostgresDAOServiceTest()
        {
            // TODO: cleanup db
            // var builder = new DbContextOptionsBuilder<BranchOfficeDbContext>();
            dbContext = new BranchOfficeDbContext();
            // // drop table even if other objects depend on it
            // // this will drop any foreign key that is referencing the Employees table or any view using it
            // // It will not drop other tables (or delete rows from them).
            // dbContext.Database.ExecuteSqlCommand("DROP TABLE if exists \"Employees\" cascade;");
            // dbContext.Database.ExecuteSqlCommand("DROP TABLE if exists \"Projects\" cascade;");
            // dbContext.Database.ExecuteSqlCommand("DROP TABLE if exists \"EmployeeHoursCollection\" cascade;");
            // dbContext.Employees.;
            // dbContext.Database.SetInitializer();
            dbContext.Database.Migrate();
            // dbContext.SaveChanges();
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
            var employees = dao.ListEmployees();
            Assert.Empty(employees);
        }

        [Fact]
        public async Task ShouldReturnAllEmployeesWhenSomeInDB()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Dwa", Email = "ola2@gmail.com" });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var employees = dao.ListEmployees();
            Assert.Equal(1, employees.Count);
        }
    }
}