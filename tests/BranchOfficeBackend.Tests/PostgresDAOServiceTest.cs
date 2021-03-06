using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace BranchOfficeBackend.Tests
{
    [Collection("do-not-run-in-parallel")]
    /// <summary>
    /// Tests the DAO layer - connection with Postgres db
    /// </summary>
    public class PostgresDAOServiceTest : IDisposable
    {
        private BranchOfficeDbContext dbContext;

        public PostgresDAOServiceTest(ITestOutputHelper testOutputHelper)
        {
            LoggingHelpers.Configure(testOutputHelper);
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
        public async Task GetOneEmployee_ShouldReturnSpecifiedEmployee_IfExists()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Dwa", Email = "ola2@gmail.com", EmployeeId = 0 });
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var employee = dao.GetOneEmployee(4);
            Assert.NotNull(employee);
            Assert.Equal("Ola AAA", employee.Name);
            Assert.Equal(4, employee.EmployeeId);
            Assert.Equal("aaaa@gmail.com", employee.Email);
        }

        [Fact]
        public async Task GetOneEmployee_ShouldReturnNull_IfNoEmployee()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Dwa", Email = "ola2@gmail.com", EmployeeId = 0 });
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var employee = dao.GetOneEmployee(55);
            Assert.Null(employee);
        }

        [Fact]
        public void GetEmployeeHoursForAnEmployee_ShouldReturnEmptyListWhenDBIsEmpty()
        {
            var dao = new PostgresDataAccessObjectService(dbContext);
            var coll = dao.GetEmployeeHoursForAnEmployee(0);
            Assert.Empty(coll);
        }

        [Fact]
        public async Task GetEmployeeHoursForAnEmployee_ShouldReturnAllEmployeesHoursWhenSomeInDB()
        {
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 0});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var coll = dao.GetEmployeeHoursForAnEmployee(0);
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
            eh.Value = 90;
            eh.TimePeriod = "02.03.2019_08.03.2019";

            var dao = new PostgresDataAccessObjectService(dbContext);
            dao.AddEmployeeHours(eh);
            
            var coll = dao.GetEmployeeHoursForAnEmployee(4);
            Assert.Equal(2, coll.Count);
            Assert.Equal(100f, coll[0].Value);
            Assert.Equal(90f, coll[1].Value);
            Assert.Equal(100, coll[0].EmployeeHoursId);
            Assert.Equal(101, coll[1].EmployeeHoursId);
        }

        [Fact]
        public async Task AddEmployeeHours_Fails_WhenAddedAlreadyWithTheSameTimePeriodEndEmployeeId()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.SaveChangesAsync();

            var eh = new EmployeeHours{ EmployeeHoursId = 101, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4};

            var dao = new PostgresDataAccessObjectService(dbContext);
            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddEmployeeHours(eh);
            } catch (Exception e) {
                Assert.Equal(typeof(ArgumentException), e.GetType());
                Assert.Equal("EmployeeHours with EmployeeId: 4 and TimePeriod 02.01.2019_08.01.2019 already exists", e.Message);
            }
            var coll = dao.GetEmployeeHoursForAnEmployee(4);
            Assert.Single(coll);
            Assert.True(exThrown);
        }

        [Fact]
        public async Task AddEmployeeHours_WhenNotEmptyTable_Many()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola 2", Email = "aaaa2@gmail.com", EmployeeId = 5 });
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 101, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 102, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 5});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            // the EmployeeHoursId is generated anyways
            dao.AddEmployeeHours(
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100f, TimePeriod = "01.01.2019_02.01.2019", EmployeeId = 4});
            dao.AddEmployeeHours(
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100f, TimePeriod = "09.01.2019_13.01.2019", EmployeeId = 4});
            dao.AddEmployeeHours(
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100f, TimePeriod = "01.01.2019_02.01.2019", EmployeeId = 5});
            
            var coll = dao.GetEmployeeHoursForAnEmployee(4);
            Assert.Equal(4, coll.Count);
        }

        [Fact]
        public async Task AddEmployeeHours_WhenEmptyTable()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var eh = new EmployeeHours();
            eh.EmployeeId = 4;
            eh.Value = 90;
            eh.TimePeriod = "02.03.2019_08.03.2019";

            var dao = new PostgresDataAccessObjectService(dbContext);
            dao.AddEmployeeHours(eh);
            
            var coll = dao.GetEmployeeHoursForAnEmployee(4);
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
            eh.Value = 90;
            eh.TimePeriod = "02.03.2019_08.03.2019";

            var dao = new PostgresDataAccessObjectService(dbContext);
            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddEmployeeHours(eh);
            } catch (Exception e) {
                Assert.Equal(typeof(ArgumentException), e.GetType());
                Assert.Equal("Employee with Id: 4 not found", e.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task AddEmployeeHours_WhenEmployeeIdNotSet()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 9 });
            await dbContext.SaveChangesAsync();

            var eh = new EmployeeHours();
            eh.Value = 90;
            eh.TimePeriod = "02.03.2019_08.03.2019";

            var dao = new PostgresDataAccessObjectService(dbContext);
            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddEmployeeHours(eh);
            } catch (Exception e) {
                Assert.Equal(typeof(ArgumentException), e.GetType());
                Assert.Equal("EmployeeId was not set", e.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task AddEmployeeHours_WhenValueInvalid()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 9 });
            await dbContext.SaveChangesAsync();

            var eh = new EmployeeHours();
            eh.EmployeeId = 4;
            eh.Value = -90;
            eh.TimePeriod = "02.03.2019_08.03.2019";

            var dao = new PostgresDataAccessObjectService(dbContext);
            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddEmployeeHours(eh);
            } catch (Exception e) {
                Assert.Equal(typeof(ArgumentException), e.GetType());
                Assert.Equal("Value < 0", e.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task AddEmployeeHours_WhenNotEmptyTable_KeepId_ConflictingKeyValues()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 100, Value = 100, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.SaveChangesAsync();

            var emp = new EmployeeHours{ EmployeeHoursId = 100, Value = 100, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4};

            var dao = new PostgresDataAccessObjectService(dbContext);
            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddEmployeeHours(emp,true);            
            } catch(Exception ex) {
                Assert.Equal(typeof(ArgumentException), ex.GetType());
                Assert.Equal("EmployeeHours with EmployeeHoursId: 100 already exists", ex.Message);
            }
            Assert.True(exThrown);
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

            var coll = dao.GetEmployeeHoursForAnEmployee(4);
            Assert.Equal(3, coll.Count);
            
            dao.DeleteEmployeeHours(100);
            
            coll = dao.GetEmployeeHoursForAnEmployee(4);
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
            
            var coll = dao.GetEmployeeHoursForAnEmployee(4);
            Assert.Equal(2, coll.Count);
            
            dao.DeleteEmployeeHours(100);
            
            coll = dao.GetEmployeeHoursForAnEmployee(4);
            Assert.Equal(2, coll.Count);
        }

        [Fact]
        public async Task EditEmployeeHours_WhenExists()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 101, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 102, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            
            var newEH = new EmployeeHours{ EmployeeHoursId = 101, Value = 777f, TimePeriod = "02.01.2019_08.01.2022", EmployeeId = 4};            
            dao.EditEmployeeHours(newEH);
            var obj = dao.GetOneEmployeeHours(101);
            Assert.NotNull(obj);
            Assert.Equal(101, obj.EmployeeHoursId);
            Assert.Equal(777f, obj.Value);
            Assert.Equal("02.01.2019_08.01.2022", obj.TimePeriod);
            Assert.Equal(4, obj.EmployeeId);
        }

        [Fact]
        public async Task EditEmployeeHours_WhenExists_ButNoEmployee()
        {
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 101, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 102, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);            
            var newEH = new EmployeeHours{ EmployeeHoursId = 101, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4};

            bool exThrown = false;
            try {
                exThrown = true;
                dao.EditEmployeeHours(newEH);
            } catch (Exception e) {
                Assert.Equal(typeof(ArgumentException), e.GetType());
                Assert.Equal("Employee with Id: 4 not found", e.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task EditEmployeeHours_WhenNotExists()
        {
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 101, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.EmployeeHoursCollection.AddAsync(
                new EmployeeHours{ EmployeeHoursId = 102, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);            
            var newEH = new EmployeeHours{ EmployeeHoursId = 666, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4};

            bool exThrown = false;
            try {
                exThrown = true;
                dao.EditEmployeeHours(newEH);
            } catch (Exception e) {
                Assert.Equal(typeof(InvalidOperationException), e.GetType());
                Assert.Equal("EmployeeHours object not found", e.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task AddEmployee_WhenNotEmptyTable()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var emp = new Employee{ Name = "Ola 2", Email = "333@gmail.com", EmployeeId = 44 };

            var dao = new PostgresDataAccessObjectService(dbContext);
            dao.AddEmployee(emp);
            
            var coll = dao.GetAllEmployees();
            Assert.Equal(2, coll.Count);
            Assert.Equal("Ola AAA", coll[0].Name);
            Assert.Equal("Ola 2", coll[1].Name);
            Assert.Equal("aaaa@gmail.com", coll[0].Email);
            Assert.Equal("333@gmail.com", coll[1].Email);
            Assert.Equal(4, coll[0].EmployeeId);
            Assert.Equal(5, coll[1].EmployeeId);
        }

        [Fact]
        public void AddEmployee_WhenEmptyTable()
        {
            var emp = new Employee{ Name = "Ola 2", Email = "333@gmail.com", EmployeeId = 44 };

            var dao = new PostgresDataAccessObjectService(dbContext);
            dao.AddEmployee(emp);
            
            var coll = dao.GetAllEmployees();
            Assert.Single(coll);
            Assert.Equal("Ola 2", coll[0].Name);
            Assert.Equal("333@gmail.com", coll[0].Email);
            Assert.Equal(1, coll[0].EmployeeId);// apparently we must start from 1
        }
        [Fact]
        public async Task AddEmployee_WhenEmployeeWithSameEmailExists()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var emp = new Employee{ Name = "Ola 2", Email = "aaaa@gmail.com", EmployeeId = 44 };

            var dao = new PostgresDataAccessObjectService(dbContext);
            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddEmployee(emp);            
            } catch(Exception ex) {
                Assert.Equal(typeof(ArgumentException), ex.GetType());
                Assert.Equal("Employee with email: aaaa@gmail.com already exists", ex.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task AddEmployee_WhenNotEmptyTable_KeepId_ConflictingKeyValues()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var emp = new Employee{ Name = "Ola 2", Email = "333@gmail.com", EmployeeId = 4 };

            var dao = new PostgresDataAccessObjectService(dbContext);
            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddEmployee(emp,true);            
            } catch(Exception ex) {
                Assert.Equal(typeof(ArgumentException), ex.GetType());
                Assert.Equal("Employee with EmployeeId: 4 already exists", ex.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task DeleteEmployee_WhenExists()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola 2", Email = "22@gmail.com", EmployeeId = 5 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);

            var coll = dao.GetAllEmployees();
            Assert.Equal(2, coll.Count);
            
            dao.DeleteEmployee(4);
            
            coll = dao.GetAllEmployees();
            Assert.Single(coll);
            Assert.Equal(5, coll[0].EmployeeId);
        }

        [Fact]
        public async Task DeleteEmployee_WhenNotExists()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola 2", Email = "22@gmail.com", EmployeeId = 5 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);

            var coll = dao.GetAllEmployees();
            Assert.Equal(2, coll.Count);
            
            dao.DeleteEmployee(477);
            
            coll = dao.GetAllEmployees();
            Assert.Equal(2, coll.Count);
        }

        [Fact]
        public async Task EditEmployee_WhenNotExists()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);            
            var newEH = new Employee{ Name = "Ola 2", Email = "22@gmail.com", EmployeeId = 5 };

            bool exThrown = false;
            try {
                exThrown = true;
                dao.EditEmployee(newEH);
            } catch (Exception e) {
                Assert.Equal(typeof(InvalidOperationException), e.GetType());
                Assert.Equal("Employee object not found", e.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task EditEmployee_WhenExists()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 5 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);            
            var newEH = new Employee{ Name = "Ola 2", Email = "22@gmail.com", EmployeeId = 5 };

            dao.EditEmployee(newEH);
            var returned = dao.GetOneEmployee(5);
            Assert.Equal(returned.EmployeeId, newEH.EmployeeId);
            Assert.Equal(returned.Name, newEH.Name);
            Assert.Equal(returned.Email, newEH.Email);
        }

        [Fact]
        public void GetAllSalaries_ShouldReturnEmptyListWhenDBIsEmpty()
        {
            var dao = new PostgresDataAccessObjectService(dbContext);
            var objects = dao.GetAllSalaries();
            Assert.Empty(objects);
        }

        [Fact]
        public async Task GetAllSalaries_ShouldReturnListWhenSomeInDB()
        {
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 1, Value = 100, TimePeriod = "some", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 2, Value = 200, TimePeriod = "some2", EmployeeId = 1 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var objects = dao.GetAllSalaries();
            Assert.Equal(2, objects.Count);
            Assert.Equal(200, objects[1].Value);
        }

        [Fact]
        public void GetSalariesForAnEmployee_ShouldReturnNull_IfNoMatching()
        {
            var dao = new PostgresDataAccessObjectService(dbContext);
            var obj = dao.GetSalariesForAnEmployee(55);
            Assert.Empty(obj);
        }

        [Fact]
        public async Task GetSalariesForAnEmployee_ShouldReturnObj_IfExists()
        {
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 1, Value = 100, TimePeriod = "some", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 2, Value = 200, TimePeriod = "some2", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 3, Value = 300, TimePeriod = "some2", EmployeeId = 2 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var objects = dao.GetSalariesForAnEmployee(1);
            Assert.Equal(2, objects.Count);
            Assert.Equal(200, objects[1].Value);
        }

        [Fact]
        public async Task GetOneSalary_ShouldReturnNull_IfNoMatch()
        {
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 1, Value = 100, TimePeriod = "some", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 2, Value = 200, TimePeriod = "some2", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 3, Value = 300, TimePeriod = "some2", EmployeeId = 2 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var obj = dao.GetOneSalary(11);
            Assert.Null(obj);
        }

        [Fact]
        public async Task GetOneSalary_ShouldReturnObj_IfExists()
        {
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 1, Value = 100, TimePeriod = "some", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 2, Value = 200, TimePeriod = "some2", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 3, Value = 300, TimePeriod = "some2", EmployeeId = 2 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);
            var obj = dao.GetOneSalary(3);
            Assert.NotNull(obj);
            Assert.Equal(300, obj.Value);
        }

        [Fact]
        public async Task AddSalary_WhenNotEmptyTable()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 2 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 1, Value = 100, TimePeriod = "some", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 2, Value = 200, TimePeriod = "some2", EmployeeId = 1 });
            await dbContext.SaveChangesAsync();

            var objToBeAdded = new Salary{ SalaryId = 3, Value = 300, TimePeriod = "some2", EmployeeId = 2 };

            var dao = new PostgresDataAccessObjectService(dbContext);
            dao.AddSalary(objToBeAdded);
            
            var coll = dao.GetAllSalaries();
            Assert.Equal(3, coll.Count);
            Assert.Equal(100f, coll[0].Value);
            Assert.Equal(200f, coll[1].Value);
            Assert.Equal(1, coll[0].EmployeeId);
            Assert.Equal(1, coll[1].EmployeeId);
            Assert.Equal(2, coll[2].EmployeeId);
        }

        [Fact]
        public async Task AddSalary_WhenEmptyTable()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.SaveChangesAsync();

            var objToBeAdded = new Salary{ SalaryId = 3, Value = 300, TimePeriod = "some2", EmployeeId = 4 };
            var dao = new PostgresDataAccessObjectService(dbContext);
            dao.AddSalary(objToBeAdded);
            
            var coll = dao.GetAllSalaries();
            Assert.Single(coll);
            Assert.Equal(4, coll[0].EmployeeId);
        }

        [Fact]
        public async Task AddSalary_WhenNoSuchEmployee()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 9 });
            await dbContext.SaveChangesAsync();

            var objToBeAdded = new Salary{ SalaryId = 3, Value = 300, TimePeriod = "some2", EmployeeId = 4 };
            var dao = new PostgresDataAccessObjectService(dbContext);

            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddSalary(objToBeAdded);
            } catch (Exception e) {
                Assert.Equal(typeof(ArgumentException), e.GetType());
                Assert.Equal("Employee with Id: 4 not found", e.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task AddSalary_WhenEmployeeIdNotSet()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 9 });
            await dbContext.SaveChangesAsync();

            var objToBeAdded = new Salary{ SalaryId = 3, Value = 300, TimePeriod = "some2" };
            var dao = new PostgresDataAccessObjectService(dbContext);

            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddSalary(objToBeAdded);
            } catch (Exception e) {
                Assert.Equal(typeof(ArgumentException), e.GetType());
                Assert.Equal("EmployeeId was not set", e.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task AddSalary_WhenNotEmptyTable_KeepId_ConflictingKeyValues()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 4 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 1, Value = 100, TimePeriod = "some", EmployeeId = 1 });
            await dbContext.SaveChangesAsync();

            var objToBeAdded = new Salary{ SalaryId = 1, Value = 300, TimePeriod = "some2", EmployeeId = 4 };

            var dao = new PostgresDataAccessObjectService(dbContext);
            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddSalary(objToBeAdded, true);       
            } catch(Exception ex) {
                Assert.Equal(typeof(ArgumentException), ex.GetType());
                Assert.Equal("Salary with SalaryId: 1 already exists", ex.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task AddSalary_Duplicate()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola AAA", Email = "aaaa@gmail.com", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 1, Value = 100, TimePeriod = "some", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 2, Value = 200, TimePeriod = "some2", EmployeeId = 1 });
            await dbContext.SaveChangesAsync();

            var objToBeAdded = new Salary{ SalaryId = 3, Value = 300, TimePeriod = "some2", EmployeeId = 1 };

            var dao = new PostgresDataAccessObjectService(dbContext);
            bool exThrown = false;
            try {
                exThrown = true;
                dao.AddSalary(objToBeAdded);       
            } catch(Exception ex) {
                Assert.Equal(typeof(ArgumentException), ex.GetType());
                Assert.Equal("Salary with TimePeriod: some2 for Employee: 1 already exists", ex.Message);
            }
            Assert.True(exThrown);
        }

        [Fact]
        public async Task DeleteSalary_WhenExists()
        {
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 1, Value = 100, TimePeriod = "some", EmployeeId = 1 });
            await dbContext.Salaries.AddAsync(new Salary{ SalaryId = 2, Value = 200, TimePeriod = "some2", EmployeeId = 1 });
            await dbContext.SaveChangesAsync();

            var dao = new PostgresDataAccessObjectService(dbContext);

            var coll = dao.GetAllSalaries();
            Assert.Equal(2, coll.Count);
            
            dao.DeleteSalary(1);
            
            coll = dao.GetAllSalaries();
            Assert.Single(coll);
            Assert.Equal(2, coll[0].SalaryId);
        }

        [Fact]
        public void DeleteSalary_WhenNotExists()
        {
            var dao = new PostgresDataAccessObjectService(dbContext);

            var coll = dao.GetAllSalaries();
            Assert.Empty(coll);
            
            dao.DeleteSalary(1);
            
            coll = dao.GetAllSalaries();
            Assert.Empty(coll);
        }

    }
}
