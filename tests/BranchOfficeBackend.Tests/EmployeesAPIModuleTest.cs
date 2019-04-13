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
        public async Task ShouldReturnJsonListOfEmployees()
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
                var result = await client.GetStringAsync("/employee/list");
                var items = JArray.Parse(result);
                Assert.Equal(2, items.Count);
            }
        }
    }
}
