using System;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BranchOfficeBackend.Tests
{
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
            }
        }

        [Fact]
        public void ShouldReturnEmptyListWhenDBIsEmpty()
        {
            var dao = new PostgresDataAccessObjectService(dbContext);
            var employees = dao.ListEmployees();
            Assert.Empty(employees);
        }
    }
}