using System.Diagnostics;
using System.Threading.Tasks;
using BranchOfficeBackend;
using Newtonsoft.Json.Linq;
using Xunit;

namespace BranchOfficeBackend.Tests
{
    /// <summary>
    /// Unit tests the API server class responsible for dealing with employees
    /// (other objects mocked)
    /// </summary>
    public class EmployeesAPIModuleTest
    {
        /// <summary>
        /// Test that API server returns 2 employees JSON list when
        /// IEmployeeRepository returns 2 employees.
        /// (DB access is mocked)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonListOfEmployees_WhenNoError()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetAllEmployees()).Returns(new System.Collections.Generic.List<WebEmployee>() {
                new WebEmployee() { Name = "John" },
                new WebEmployee() { Name = "Jane" }
            });
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employees/list");
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);    
                var jsonString = await response.Content.ReadAsStringAsync();
                var items = JArray.Parse(jsonString);
                Assert.Equal(2, items.Count);                            
            }
        }

        [Fact]
        public async Task ShouldNotReturnJsonEmployee_WhenNotExists()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetEmployee(33)).Returns(
                new WebEmployee() { Name = "John", ID = 33 }
            );
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employees/100");
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);      
            }
        }

        [Fact]
        public async Task ShouldReturnJsonEmployee_WhenExists()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetEmployee(33)).Returns(
                new WebEmployee() { Name = "John", ID = 33 }
            );
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employees/33");
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);  
                var jsonString = await response.Content.ReadAsStringAsync();
                var actual = JObject.Parse(jsonString);
                Debug.WriteLine(actual);
                var expected = new JObject{
                    {"name", "John"},
                    {"id", 33 },
                    {"isManager", false }
                }; 
                Debug.WriteLine(actual);
                Assert.True(JToken.DeepEquals(expected, actual));                               
            }
        }
    }
}
