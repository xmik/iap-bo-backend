using System.Diagnostics;
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
    }
}