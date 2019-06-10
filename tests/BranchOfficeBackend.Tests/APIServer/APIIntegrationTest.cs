using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BranchOfficeBackend;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace BranchOfficeBackend.Tests
{
    /// <summary>
    /// No mocks
    /// </summary>
    [Collection("do-not-run-in-parallel")]
    public class APIIntegrationTest : IDisposable
    {
        private BranchOfficeDbContext dbContext;

        public APIIntegrationTest(ITestOutputHelper testOutputHelper)
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
        public async Task GetEmployeesList_WhenNoData()
        {
            using(var testServer = new TestServerBuilder()
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employees/list");
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);    
                var jsonString = await response.Content.ReadAsStringAsync();
                var items = JArray.Parse(jsonString);
                Assert.Empty(items);                     
            }
        }

        [Fact]
        public async Task GetEmployeesList_WhenSomeData()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Dwa", Email = "ola2@gmail.com" });
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Trzy", Email = "ola3@gmail.com" });
            await dbContext.SaveChangesAsync();
            using(var testServer = new TestServerBuilder()
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
        public async Task GetEmployee_WhenNoData()
        {
            using(var testServer = new TestServerBuilder()
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employees/100");
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);    
            }
        }

        [Fact]
        public async Task GetEmployee_WhenSomeData()
        {
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Dwa", Email = "ola2@gmail.com" });
            await dbContext.Employees.AddAsync(new Employee{ Name = "Ola Trzy", Email = "ola3@gmail.com" });
            await dbContext.SaveChangesAsync();
            using(var testServer = new TestServerBuilder()
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employees/1");
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);   
                var jsonString = await response.Content.ReadAsStringAsync();
                var actual = JObject.Parse(jsonString);
                Debug.WriteLine(actual);
                var expected = new JObject{
                    {"name", "Ola Dwa"},
                    {"id", 1 },
                    {"isManager", false },
                    {"email", "ola2@gmail.com" }
                }; 
                Debug.WriteLine(actual);
                Assert.True(JToken.DeepEquals(expected, actual));             
            }
        }

        [Fact]
        public async Task GetEHForAnEmployee_WhenNoData()
        {
            using(var testServer = new TestServerBuilder()
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employee_hours/list/10");
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);    
            }
        }

        [Fact]
        public async Task GetEH_WhenNoData()
        {
            using(var testServer = new TestServerBuilder()
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/employee_hours/10");
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);    
            }
        }

        [Fact]
        public async Task GetSalaries_WhenNoData()
        {
            using(var testServer = new TestServerBuilder()
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/salaries/list");
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);    
            }
        }

        [Fact]
        public async Task GetSalariesForAnEmployee_WhenNoData()
        {
            using(var testServer = new TestServerBuilder()
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.GetAsync("/api/salaries/list/11");
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);    
            }
        }

        [Fact]
        public async Task PostEmployee_EmailNotSet()
        {
            using(var testServer = new TestServerBuilder()
                .Build())
            {
                var client = testServer.CreateClient();
                var myJson = "{ 'employeeId': 33 }";
                HttpContent requestContent = new StringContent(myJson, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/employees", requestContent);
                Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);    
                var jsonString = await response.Content.ReadAsStringAsync();
                Assert.Equal("Problem when adding the object to database: Email was empty", jsonString);  
            }
        }

        [Fact]
        public async Task PostEmployee_Success()
        {
            using(var testServer = new TestServerBuilder()
                .Build())
            {
                var client = testServer.CreateClient();
                var myJson = "{ 'email': '123@gmail.com', 'name': 'ewa' }";
                HttpContent requestContent = new StringContent(myJson, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/employees", requestContent);
                Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);    
            }
        }
    }
}