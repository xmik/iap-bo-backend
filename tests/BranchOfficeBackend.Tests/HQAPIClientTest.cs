using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using Xunit;

namespace BranchOfficeBackend.Tests
{
    public class HQAPIClientTest
    {
        [Fact]
        public void ShouldReturnNiceUrl_WhenBaseUrlDoesNotEndWithSlash()
        {
            HQAPIClient client = new HQAPIClient("http://test.com:1234");
            string url = client.BuildUrl("/api/branch_offices/list");
            Assert.Equal("http://test.com:1234/api/branch_offices/list", url);
        }

        [Fact]
        public void ShouldReturnNiceUrl_WhenBaseUrlEndsWithSlash()
        {
            HQAPIClient client = new HQAPIClient("http://test.com:1234/");
            string url = client.BuildUrl("/api/branch_offices/list");
            Assert.Equal("http://test.com:1234/api/branch_offices/list", url);
        }

        /// <summary>
        /// Test that API server returns 1 branch office
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonOneBranchOffice()
        {
            string baseUrl = "http://test.com:1234";

            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(baseUrl + "/api/branch_offices/*")
                    .Respond("application/json",
                    "{'name' : 'BranchOffice1'}"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(baseUrl, client)) 
            {
                var branchOffice = await hqClient.GetBranchOffice(999);
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Equal("BranchOffice1", branchOffice.Name);
            }
        }

        /// <summary>
        /// Test that API server returns a list of branch offices
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonListOfBranchOffices()
        {
            string baseUrl = "http://test.com:1234";

            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(baseUrl + "/api/branch_offices")
                    .Respond("application/json",
                    "{ 'branch_offices': [ {'name' : 'BranchOffice1'}, {'name' : 'BranchOffice999'} ] }"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(baseUrl, client)) 
            {
                var branchOffices = await hqClient.ListBranchOffices();
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Equal(2, branchOffices.Count);
                Assert.Equal("BranchOffice1", branchOffices[0].Name);
                Assert.Equal("BranchOffice999", branchOffices[1].Name);
            }
        }

        /// <summary>
        /// Test that API server returns 1 employee
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonOneEmployee()
        {
            string baseUrl = "http://test.com:1234";

            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(baseUrl + "/api/employees/*")
                    .Respond("application/json",
                    "{'name' : 'Jan Kow', 'id': 123, 'email': 'jank@gmail.com', 'isManager': false }"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(baseUrl, client)) 
            {
                var employee = await hqClient.GetEmployee(123);
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Equal(123, employee.ID);
                Assert.Equal("Jan Kow", employee.Name);
                Assert.Equal("jank@gmail.com", employee.Email);
                Assert.Equal(false, employee.IsManager);
            }
        }

        /// <summary>
        /// Test that API server returns a list of employees
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonListOfEmployees()
        {
            string baseUrl = "http://test.com:1234";

            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(baseUrl + "/api/employees/list/*")
                    .Respond("application/json",
                    "{ 'employees': [ {'id' : 1}, {'id' : 123} ] }"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(baseUrl, client)) 
            {
                var employees = await hqClient.ListEmployees(100);
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Equal(2, employees.Count);
                Assert.Equal(1, employees[0].ID);
                Assert.Equal(123, employees[1].ID);
            }
        }

        /// <summary>
        /// Test that API server returns a list of salaries
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonListOfSalaries()
        {
            string baseUrl = "http://test.com:1234";

            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(baseUrl + "/api/salaries/list/*")
                    .Respond("application/json",
                    "{ 'salaries': [ {'id' : 98, 'timePeriod': '2019-Jun-15_2019-Jun-21', 'value': 300, 'employeeId': 1 }," +
                    "{'id' : 99, 'timePeriod': '2019-Jun-22_2019-Jun-28', 'value': 320, 'employeeId': 1}" +
                    " ] }"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(baseUrl, client)) 
            {
                var salaries = await hqClient.ListSalariesForEmployee(1);
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Equal(2, salaries.Count);
                Assert.Equal(98, salaries[0].ID);
                Assert.Equal(99, salaries[1].ID);
                Assert.Equal("2019-Jun-15_2019-Jun-21", salaries[0].TimePeriod);
                Assert.Equal("2019-Jun-22_2019-Jun-28", salaries[1].TimePeriod);
                Assert.Equal(1, salaries[0].EmployeeID);
                Assert.Equal(1, salaries[1].EmployeeID);
                Assert.Equal(300f, salaries[0].Value);
                Assert.Equal(320f, salaries[1].Value);
            }
        }
    }
}