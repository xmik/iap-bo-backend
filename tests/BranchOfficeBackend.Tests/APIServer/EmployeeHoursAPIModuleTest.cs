using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BranchOfficeBackend;
using Newtonsoft.Json.Linq;
using Xunit;

namespace BranchOfficeBackend.Tests
{
    /// <summary>
    /// Unit tests the API server class responsible for dealing with employeeHours
    /// (other objects mocked)
    /// </summary>
    public class EmployeeHoursAPIModuleTest
    {
        [Fact]
        public async Task GetCollectionOfEHObj_ShouldReturnJsonList_WhenFound()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetAllEmployeeHours(0)).Returns(new System.Collections.Generic.List<WebEmployeeHours>() {
                new WebEmployeeHours() { Value = 15f, TimePeriod = "20.1.2019-26.01.2019", EmployeeId = 0, Id = 0 },
                new WebEmployeeHours() { Value = 10f, TimePeriod = "27.01.2019-02.02.2019", EmployeeId = 0, Id = 1 }
            });
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employee_hours/list/0");
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);    
                var jsonString = await response.Content.ReadAsStringAsync();
                var items = JArray.Parse(jsonString);
                Assert.Equal(2, items.Count);                     
            }
        }

        [Fact]
        public async Task GetCollectionOfEHObj_ShouldReturn404_WhenNotFound()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetAllEmployeeHours(0)).Returns(new System.Collections.Generic.List<WebEmployeeHours>() {
                new WebEmployeeHours() { Value = 15f, TimePeriod = "20.1.2019-26.01.2019", EmployeeId = 0, Id = 0 },
                new WebEmployeeHours() { Value = 10f, TimePeriod = "27.01.2019-02.02.2019", EmployeeId = 0, Id = 1 }
            });
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employee_hours/list/999");
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);      
            }
        }
        [Fact]
        public async Task GetOneEHObj_ShouldReturnOneObj_WhenFound()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetOneEmployeeHours(77)).Returns(
                new WebEmployeeHours() { Value = 15f, TimePeriod = "20.1.2019-26.01.2019", EmployeeId = 0, Id = 77 }
            );
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employee_hours/77");
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
                var jsonString = await response.Content.ReadAsStringAsync();
                var actual = JObject.Parse(jsonString);
                Debug.WriteLine(actual);
                var expected = new JObject{
                    {"value", 15.0 },
                    {"id", 77 },
                    {"timePeriod", "20.1.2019-26.01.2019" },
                    {"employeeId", 0 },
                    {"hoursCount", 0}
                }; 
                Assert.True(JToken.DeepEquals(expected, actual));                    
            }
        }
        [Fact]
        public async Task GetOneEHObj_ShouldReturn404_WhenNotFound()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetOneEmployeeHours(77)).Returns(
                new WebEmployeeHours() { Value = 15, TimePeriod = "20.1.2019-26.01.2019", EmployeeId = 0, Id = 77 }
            );
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employee_hours/1111111111111");
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task PostOneEHObj_ShouldSucceed_UnitTest()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.AddEmployeeHours(Moq.It.IsAny<WebEmployeeHours>()));
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var myJson = "{ 'employeeId': 33 }";
                HttpContent requestContent = new StringContent(myJson, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/employee_hours", requestContent);
                Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);   
            }
            mock.Verify(m => m.AddEmployeeHours(Moq.It.IsAny<WebEmployeeHours>() ), Moq.Times.Once);
        }

        [Fact]
        public async Task PostOneEHObj_ShouldFail_WhenAddingToDBThrowsException()
        {
            var mockDB = new Moq.Mock<IDataAccessObjectService>();
            mockDB.Setup(m => m.AddEmployeeHours(Moq.It.IsAny<EmployeeHours>(),false )).Throws<ArgumentException>();
            using(var testServer = new TestServerBuilder()
                .WithMock<IDataAccessObjectService>(typeof(IDataAccessObjectService), mockDB)
                .Build())
            {
                var client = testServer.CreateClient();
                var myJson = "{ 'employeeId': 33 }";
                HttpContent requestContent = new StringContent(myJson, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/employee_hours", requestContent);
                Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);   
            }
            mockDB.Verify(m => m.AddEmployeeHours(Moq.It.IsAny<EmployeeHours>(),false ), Moq.Times.Once);
        }
        [Fact]
        public async Task DeleteOneEHObj_ShouldReturn404_WhenNotFound()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetOneEmployeeHours(77)).Returns(
                new WebEmployeeHours() { Value = 15, TimePeriod = "20.1.2019-26.01.2019", EmployeeId = 0, Id = 77 }
            );
            mock.Setup(m => m.GetOneEmployeeHours(111111111)).Returns(
                (WebEmployeeHours)null 
            );
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.DeleteAsync("/api/employee_hours/111111111");
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task DeleteOneEHObj_ShouldReturn202_WhenFound()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetOneEmployeeHours(77)).Returns(
                new WebEmployeeHours() { Value = 15, TimePeriod = "20.1.2019-26.01.2019", EmployeeId = 1, Id = 77 }
            );
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.DeleteAsync("/api/employee_hours/77");
                Assert.Equal(System.Net.HttpStatusCode.Accepted, response.StatusCode);
            }
        }
        [Fact]
        public async Task DeleteOneEHObj_ShouldReturn202_WhenFound_RealWebService()
        {
            var mock = new Moq.Mock<IDataAccessObjectService>();
            mock.Setup(m => m.DeleteEmployeeHours(77));
            mock.Setup(m => m.GetOneEmployeeHours(77)).Returns(
                    new EmployeeHours{EmployeeId = 1, EmployeeHoursId = 77, HoursCount = 12, TimePeriod = "aa", Value = 100});
            using(var testServer = new TestServerBuilder()
                .WithMock<IDataAccessObjectService>(typeof(IDataAccessObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.DeleteAsync("/api/employee_hours/77");
                Assert.Equal(System.Net.HttpStatusCode.Accepted, response.StatusCode);
            }
            mock.Verify(x => x.DeleteEmployeeHours(77), Moq.Times.Once);
        }
        [Fact]
        public async Task EditOneEHObj_ShouldReturn202_WhenFound()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetOneEmployeeHours(77)).Returns(
                new WebEmployeeHours() { Value = 15, TimePeriod = "20.1.2019-26.01.2019", EmployeeId = 1, Id = 77 }
            );
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var myJson = "{ 'employeeId': 33, 'Value': 15, 'TimePeriod': '20.1.2019-26.01.2019', 'HoursCount': 99, 'id': 77 }";
                HttpContent requestContent = new StringContent(myJson, Encoding.UTF8, "application/json");
                var response = await client.PutAsync("/api/employee_hours", requestContent);
                Assert.Equal(System.Net.HttpStatusCode.Accepted, response.StatusCode);
            }
        }
        [Fact]
        public async Task EditOneEHObj_ShouldReturn404_WhenNotFound()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetOneEmployeeHours(77)).Returns(
                (WebEmployeeHours)null 
            );
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var myJson = "{ 'employeeId': 33, 'Value': 15, 'TimePeriod': '20.1.2019-26.01.2019', 'HoursCount': 99, 'id': 77 }";
                HttpContent requestContent = new StringContent(myJson, Encoding.UTF8, "application/json");
                var response = await client.PutAsync("/api/employee_hours", requestContent);
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task EditOneEHObj_ShouldFail_WhenEditingInDBThrowsException()
        {
            var mockDB = new Moq.Mock<IDataAccessObjectService>();
            mockDB.Setup(m => m.GetOneEmployeeHours(77)).
                Returns(new EmployeeHours{ EmployeeHoursId = 77, Value = 100f, TimePeriod = "02.01.2019_08.01.2019", EmployeeId = 4});
            mockDB.Setup(m => m.GetOneEmployee(33)).
                Returns((Employee)null);
            mockDB.Setup(m => m.EditEmployeeHours(Moq.It.IsAny<EmployeeHours>() )).Throws<ArgumentException>();
            using(var testServer = new TestServerBuilder()
                .WithMock<IDataAccessObjectService>(typeof(IDataAccessObjectService), mockDB)
                .Build())
            {
                var client = testServer.CreateClient();
                var myJson = "{ 'employeeId': 33, 'Value': 15, 'TimePeriod': '20.1.2019-26.01.2019', 'HoursCount': 99, 'id': 77 }";
                HttpContent requestContent = new StringContent(myJson, Encoding.UTF8, "application/json");
                var response = await client.PutAsync("/api/employee_hours", requestContent);
                Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);   
            }
        }
    }
}