using System.Threading.Tasks;
using BranchOfficeBackend;
using Newtonsoft.Json.Linq;
using Xunit;

namespace BranchOfficeBackend.Tests
{
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
            var mock = new Moq.Mock<IEmployeeRepository>();
            mock.Setup(m => m.GetAllEmployees()).Returns(new System.Collections.Generic.List<WebEmployee>() {
                new WebEmployee() { Name = "John" },
                new WebEmployee() { Name = "Jane" }
            });
            using(var testServer = new TestServerBuilder()
                .WithMock<IEmployeeRepository>(typeof(IEmployeeRepository), mock)
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