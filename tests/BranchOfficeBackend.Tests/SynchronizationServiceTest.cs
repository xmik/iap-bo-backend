using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace BranchOfficeBackend.Tests
{
    [Collection("do-not-run-in-parallel")]
    public class SynchronizationServiceTest: IDisposable
    {
        private BranchOfficeDbContext dbContext;

        public SynchronizationServiceTest (ITestOutputHelper testOutputHelper) 
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

        [Fact(Timeout = 500)]
        public async Task SynchronizeDoesNotThrowExceptionWhenNoHQServer()
        {
            var cs = CommonHelpers.MockConfServ(false);
            var hqApiClient = new Moq.Mock<IHQAPIClient>();            
            var dao = new Moq.Mock<IDataAccessObjectService>();
            var ss = new SynchronizatorService(hqApiClient.Object, cs, dao.Object);
            await ss.Synchronize();
            hqApiClient.Verify(m => m.ListEmployees(0), Moq.Times.Once);
            // ListEmployees did not succeed, so no new http requests are handled
            hqApiClient.Verify(m => m.ListSalariesForEmployee(0), Moq.Times.Never);
            ss.Dispose();
        }

        [Fact(Timeout = 500)]
        public async Task SynchronizeDoesNotThrowExceptionWhenNoHQServer_ManyTimesSequentially()
        {
            var cs = CommonHelpers.MockConfServ(false);
            var hqApiClient = new Moq.Mock<IHQAPIClient>();            
            var dao = new Moq.Mock<IDataAccessObjectService>();
            var ss = new SynchronizatorService(hqApiClient.Object, cs, dao.Object);
            await ss.Synchronize();
            await ss.Synchronize();
            await ss.Synchronize();
            await ss.Synchronize();
            hqApiClient.Verify(m => m.ListEmployees(0), Moq.Times.Exactly(4));
            // ListEmployees did not suceed, so no new http requests are handled
            hqApiClient.Verify(m => m.ListSalariesForEmployee(0), Moq.Times.Never());
            ss.Dispose();
        }

        [Fact]
        public async Task Synchronize_Works_StartWithEmplyDb_HQEmpty()
        {
            var cs = CommonHelpers.MockConfServ(false);
            var hqApiClient = new Moq.Mock<IHQAPIClient>();       
            // no employees in HQ
            hqApiClient.Setup(m => m.ListEmployees(cs.GetBranchOfficeId())).Returns(
                Task.FromResult(new List<HQEmployee>())                
            );         
            // no salaries in HQ
            hqApiClient.Setup(m => m.ListSalariesForEmployee(Moq.It.IsAny<int>())).Returns(
                Task.FromResult(new List<HQSalary>())                
            );

            var dao = new PostgresDataAccessObjectService(dbContext);
            var ss = new SynchronizatorService(hqApiClient.Object, cs, dao);
            await ss.Synchronize();
            hqApiClient.Verify(m => m.ListEmployees(0), Moq.Times.Once);
            // ListEmployees did not succeed, so no new http requests are handled
            hqApiClient.Verify(m => m.ListSalariesForEmployee(Moq.It.IsAny<int>()), Moq.Times.Never);
            ss.Dispose();

            var emps = dao.GetAllEmployees();
            var eh = dao.GetAllEmployeeHours();
            Assert.Equal(0,emps.Count);
            Assert.Equal(0,eh.Count);
        }

        /// <summary>
        /// First: 0 employees in BO, then: 2 employees added to BO
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Synchronize_Works_StartWithEmplyDb_HQNotEmpty()
        {
            var cs = CommonHelpers.MockConfServ(false);
            var hqApiClient = new Moq.Mock<IHQAPIClient>();       
            
            var hqEmp1 = new HQEmployee{Name = "Jan K", Email = "jank@gmail.com", IsManager = false, ID = 1};
            var hqEmp2 = new HQEmployee{Name = "Ela K", Email = "elak@gmail.com", IsManager = true, ID = 2};
            var hqEmps = new List<HQEmployee>();
            hqEmps.Add(hqEmp1);
            hqEmps.Add(hqEmp2);

            hqApiClient.Setup(m => m.ListEmployees(cs.GetBranchOfficeId())).Returns(
                Task.FromResult(hqEmps)                
            );         
            // no salaries in HQ
            hqApiClient.Setup(m => m.ListSalariesForEmployee(Moq.It.IsAny<int>())).Returns(
                Task.FromResult(new List<HQSalary>())                
            );

            var dao = new PostgresDataAccessObjectService(dbContext);
            var ss = new SynchronizatorService(hqApiClient.Object, cs, dao);
            await ss.Synchronize();
            hqApiClient.Verify(m => m.ListEmployees(0), Moq.Times.Once);
            // 1 for each hq employee
            hqApiClient.Verify(m => m.ListSalariesForEmployee(Moq.It.IsAny<int>()), Moq.Times.Exactly(2));
            ss.Dispose();

            var emps = dao.GetAllEmployees();
            var eh = dao.GetAllEmployeeHours();
            Assert.Equal(2,emps.Count);
            Assert.Equal("jank@gmail.com",emps[0].Email);
            Assert.Equal("elak@gmail.com",emps[1].Email);
            Assert.Equal(0,eh.Count);
        }

        /// <summary>
        /// First: 2 employees in BO, then: 1 employees added to BO and 1 deleted from BO
        /// EmployeeHours are preserved for the preserved users 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Synchronize_Works_StartWithNotEmplyDb_HQNotEmpty()
        {
            var cs = CommonHelpers.MockConfServ(false);
            var hqApiClient = new Moq.Mock<IHQAPIClient>();       
            
            // setup HQ state
            var hqEmp1 = new HQEmployee{Name = "Jan K", Email = "jank@gmail.com", IsManager = false, ID = 1};
            var hqEmp2 = new HQEmployee{Name = "Ela K", Email = "elak@gmail.com", IsManager = true, ID = 2};
            var hqEmps = new List<HQEmployee>();
            hqEmps.Add(hqEmp1);
            hqEmps.Add(hqEmp2);

            hqApiClient.Setup(m => m.ListEmployees(cs.GetBranchOfficeId())).Returns(
                Task.FromResult(hqEmps)                
            );         
            // no salaries in HQ
            hqApiClient.Setup(m => m.ListSalariesForEmployee(Moq.It.IsAny<int>())).Returns(
                Task.FromResult(new List<HQSalary>())                
            );

            var dao = new PostgresDataAccessObjectService(dbContext);
            // setup BO state
            var boEmp1 = new Employee{ Name = "BO Employee1", Email = "aaaa@gmail.com", EmployeeId = 4 };
            var boEmp2 = new Employee{ Name = "Ela K", Email = "elak@gmail.com", EmployeeId = 5, IsManager = true };
            dao.AddEmployee(boEmp1,true);
            dao.AddEmployee(boEmp2,true);
            var eh1 = new EmployeeHours{ EmployeeHoursId = 102, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4, HoursCount = 10};
            var eh2 = new EmployeeHours{ EmployeeHoursId = 103, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4, HoursCount = 10};
            var eh3 = new EmployeeHours{ EmployeeHoursId = 104, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4, HoursCount = 10};
            var eh4 = new EmployeeHours{ EmployeeHoursId = 105, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 5, HoursCount = 10};
            dao.AddEmployeeHours(eh1,true);
            dao.AddEmployeeHours(eh2,true);
            dao.AddEmployeeHours(eh3,true);
            dao.AddEmployeeHours(eh4,true);

            var ss = new SynchronizatorService(hqApiClient.Object, cs, dao);
            await ss.Synchronize();
            hqApiClient.Verify(m => m.ListEmployees(0), Moq.Times.Once);
            // 1 for each hq employee
            hqApiClient.Verify(m => m.ListSalariesForEmployee(Moq.It.IsAny<int>()), Moq.Times.Exactly(2));
            ss.Dispose();

            var emps = dao.GetAllEmployees();
            var eh = dao.GetAllEmployeeHours();
            Assert.Equal(2,emps.Count);
            Assert.Equal("jank@gmail.com",emps[1].Email);
            Assert.Equal("elak@gmail.com",emps[0].Email);
            Assert.Equal(1,eh.Count);
            Assert.Equal(105,eh[0].EmployeeHoursId);
        }

        // [Fact(Timeout = 500)]
        // public void SynchronizeDoesNotThrowExceptionWhenNoHQServer_ManyTimesParallely()
        // {
        //     var cs = CommonHelpers.MockConfServ(false);
        //     var hqApiClient = new Moq.Mock<IHQAPIClient>();            
        //     var dao = new Moq.Mock<IDataAccessObjectService>();
        //     var ss = new SynchronizatorService(hqApiClient.Object, cs, dao.Object);
        //     ss.Synchronize();
        //     ss.Synchronize();
        //     ss.Synchronize();
        //     ss.Synchronize();
        //     hqApiClient.Verify(m => m.ListEmployees(0), Moq.Times.Exactly(1));
        //     // ListEmployees did not suceed, so no new http requests are handled
        //     hqApiClient.Verify(m => m.ListSalariesForEmployee(0), Moq.Times.Never);
        //     ss.Dispose();
        // }
    }
}