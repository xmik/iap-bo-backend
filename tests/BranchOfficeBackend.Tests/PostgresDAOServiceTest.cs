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
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 0});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var coll = dao.GetAllEmployeeHours(0);
            Assert.Single(coll);
        }

        [Fact]
        public async Task GetOneEmployeeHours_ShouldReturnNull_IfNoEmployeeHours()
        {
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 101, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 102, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var obj = dao.GetOneEmployeeHours(55);
            Assert.Null(obj);
        }

        [Fact]
        public async Task GetOneEmployeeHours_ShouldReturnObj_IfExists()
        {
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 101, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 102, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var obj = dao.GetOneEmployeeHours(101);
            Assert.NotNull(obj);
            Assert.Equal(101, obj.EmployeeHoursId);
        }

        [Fact]
        public async Task AddEmployeeHours_WhenNotEmptyTable()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.SaveChangesAsync();

            var eh = new EmployeeHours();
            eh.EmployeeId = 4;
            eh.HoursCount = 90;
            eh.Value = 90;
            eh.TimePeriod = "02.03.2019_08.03.2019";

            var dao = new PostgresDataAccessObjectService(dbContext);
            dao.AddEmployeeHours(eh);
            
            var coll = dao.GetAllEmployeeHours(4);
            Assert.Equal(2, coll.Count);
            Assert.Equal(100f, coll[0].Value);
            Assert.Equal(90f, coll[1].Value);
            Assert.Equal(100, coll[0].EmployeeHoursId);
            Assert.Equal(101, coll[1].EmployeeHoursId);
        }

        [Fact]
        public async Task AddEmployeeHours_WhenEmptyTable()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var eh = new EmployeeHours();
            eh.EmployeeId = 4;
            eh.HoursCount = 90;
            eh.Value = 90;
            eh.TimePeriod = "02.03.2019_08.03.2019";

            var dao = new PostgresDataAccessObjectService(dbContext);
            dao.AddEmployeeHours(eh);
            
            var coll = dao.GetAllEmployeeHours(4);
            Assert.Single(coll);
            Assert.Equal(90f, coll[0].Value);
            // apparently we must start from 1
            Assert.Equal(1, coll[0].EmployeeHoursId);
        }

        [Fact]
        public async Task AddEmployeeHours_WhenNoSuchEmployee()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 9 });
            await dbContext.SaveChangesAsync();

            var eh = new EmployeeHours();
            eh.EmployeeId = 4;
            eh.HoursCount = 90;
            eh.Value = 90;
            eh.TimePeriod = "02.03.2019_08.03.2019";

            var dao = new PostgresDataAccessObjectService(dbContext);
            try {
                dao.AddEmployeeHours(eh);
            } catch (Exception e) {
                Assert.Equal(typeof(ArgumentException), e.GetType());
                Assert.Equal("Employee with Id: 4 not found", e.Message);
            }
        }

        [Fact]
        public async Task AddEmployeeHours_WhenEmployeeIdNotSet()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 9 });
            await dbContext.SaveChangesAsync();

            var eh = new EmployeeHours();
            eh.HoursCount = 90;
            eh.Value = 90;
            eh.TimePeriod = "02.03.2019_08.03.2019";

            var dao = new PostgresDataAccessObjectService(dbContext);
            try {
                dao.AddEmployeeHours(eh);
            } catch (Exception e) {
                Assert.Equal(typeof(ArgumentException), e.GetType());
                Assert.Equal("EmployeeId was not set", e.Message);
            }
        }

        [Fact]
        public async Task AddEmployeeHours_WhenHoursCountInvalid()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 9 });
            await dbContext.SaveChangesAsync();

            var eh = new EmployeeHours();
            eh.EmployeeId = 4;
            eh.HoursCount = -90;
            eh.Value = 90;
            eh.TimePeriod = "02.03.2019_08.03.2019";

            var dao = new PostgresDataAccessObjectService(dbContext);
            try {
                dao.AddEmployeeHours(eh);
            } catch (Exception e) {
                Assert.Equal(typeof(ArgumentException), e.GetType());
                Assert.Equal("HoursCount < 0", e.Message);
            }
        }

        [Fact]
        public async Task DeleteEmployeeHours_WhenExists()
        {
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 101, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 102, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);

            var coll = dao.GetAllEmployeeHours(4);
            Assert.Equal(3, coll.Count);
            
            dao.DeleteEmployeeHours(100);
            
            coll = dao.GetAllEmployeeHours(4);
            Assert.Equal(2, coll.Count);
        }

        [Fact]
        public async Task DeleteEmployeeHours_WhenNotExists()
        {
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 101, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 102, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            
            var coll = dao.GetAllEmployeeHours(4);
            Assert.Equal(2, coll.Count);
            
            dao.DeleteEmployeeHours(100);
            
            coll = dao.GetAllEmployeeHours(4);
            Assert.Equal(2, coll.Count);
        }

    }
}
