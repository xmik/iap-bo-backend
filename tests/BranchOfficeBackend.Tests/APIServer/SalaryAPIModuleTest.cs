using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BranchOfficeBackend;
using Newtonsoft.Json.Linq;
using Xunit;

namespace BranchOfficeBackend.Tests
{
    public class SalaryAPIModuleTest
    {
        [Fact]
        public async Task GetAll_ShouldReturnList_WhenNoError()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetAllSalaries()).Returns(new System.Collections.Generic.List<WebSalary>() {
                new WebSalary() { ID = 1, TimePeriod = "01.02.2019-09.02.2019", EmployeeId = 3, Value = 100f },
                new WebSalary() { ID = 2, TimePeriod = "01.02.2019-09.02.2019", EmployeeId = 3, Value = 1044f }
            });
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/salaries/list");
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
                var jsonString = await response.Content.ReadAsStringAsync();
                var items = JArray.Parse(jsonString);
                Assert.Equal(2, items.Count);
            }
        }
        [Fact]
        public async Task GetForEmployee_ShouldReturn404_WhenNoData()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetSalariesForAnEmployee(1)).Returns((List<WebSalary>)(null));
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/salaries/list/1");
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task GetForEmployee_ShouldReturn200_WhenExists()
        {
            var mock = new Moq.Mock<IWebObjectService>();
            mock.Setup(m => m.GetSalariesForAnEmployee(3)).Returns(new System.Collections.Generic.List<WebSalary>() {
                new WebSalary() { ID = 1, TimePeriod = "01.02.2019-09.02.2019", EmployeeId = 3, Value = 100f },
                new WebSalary() { ID = 2, TimePeriod = "01.02.2019-09.02.2019", EmployeeId = 3, Value = 1044f }
            });
            using(var testServer = new TestServerBuilder()
                .WithMock<IWebObjectService>(typeof(IWebObjectService), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/salaries/list/3");
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
                var jsonString = await response.Content.ReadAsStringAsync();
                var items = JArray.Parse(jsonString);
                Assert.Equal(2, items.Count);
            }
        }
    }
}
