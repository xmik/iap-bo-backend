using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BranchOfficeBackend.Tests
{
    public class SynchronizationServiceTest
    {
        public SynchronizationServiceTest (ITestOutputHelper testOutputHelper) 
        { 
            LoggingHelpers.Configure(testOutputHelper);
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