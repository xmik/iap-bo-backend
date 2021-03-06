using System;
using System.Net;
using System.Net.Http;
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
            HQAPIClient client = new HQAPIClient(CommonHelpers.MockConfServ());
            string url = client.BuildUrl("/api/branch_offices/list");
            Assert.Equal("http://localhost:1234/api/branch_offices/list", url);
        }

        [Fact]
        public void ShouldReturnNiceUrl_WhenBaseUrlEndsWithSlash()
        {
            HQAPIClient client = new HQAPIClient(CommonHelpers.MockConfServ());
            string url = client.BuildUrl("/api/branch_offices/list");
            Assert.Equal("http://localhost:1234/api/branch_offices/list", url);
        }

        /// <summary>
        /// Test that API server returns 1 branch office
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonOneBranchOffice()
        {
            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(CommonHelpers.baseUrl + "/api/branch_offices/*")
                    .Respond("application/json",
                    "{'id': 1, 'name' : 'BranchOffice1'}"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(CommonHelpers.MockConfServ(), client)) 
            {
                var branchOffice = await hqClient.GetBranchOffice(999);
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Equal("BranchOffice1", branchOffice.Name);
                Assert.Equal(1, branchOffice.ID);
            }
        }

        [Fact]
        public async Task ListBranchOffices_ShouldReturnEmptyListOfBranchOffices_WhenNoBO()
        {
            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(CommonHelpers.baseUrl + "/api/branch_offices")
                    .Respond("application/json",
                    "[]"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(CommonHelpers.MockConfServ(), client)) 
            {
                var branchOffices = await hqClient.ListBranchOffices();
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Empty(branchOffices);
            }
        }

        /// <summary>
        /// Test that API server returns a list of branch offices
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonListOfBranchOffices()
        {
            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(CommonHelpers.baseUrl + "/api/branch_offices")
                    .Respond("application/json",
                    "{ 'branch_offices': [ {'id' : 1, 'name' : 'BranchOffice1'}, {'id' : 2, 'name' : 'BranchOffice2'} ] }"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(CommonHelpers.MockConfServ(), client)) 
            {
                var branchOffices = await hqClient.ListBranchOffices();
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Equal(2, branchOffices.Count);
                Assert.Equal(1, branchOffices[0].ID);
                Assert.Equal(2, branchOffices[1].ID);
                Assert.Equal("BranchOffice1", branchOffices[0].Name);
                Assert.Equal("BranchOffice2", branchOffices[1].Name);
            }
        }

        /// <summary>
        /// Test that API server returns 1 employee
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonOneEmployee()
        {
            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(CommonHelpers.baseUrl + "/api/employees/*")
                    .Respond("application/json",
                    "{'employee_id': 123, 'name' : 'Jan Kow', 'email': 'jank@gmail.com', 'date_of_birth': '1996-01-01', "+
                    " 'isManager': false, 'pay': 30.0,  'branch_office_id': 2 }"); // Respond with JSON
                    //"{'name' : 'Jan Kow', 'id': 123, 'email': 'jank@gmail.com', 'isManager': false }"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(CommonHelpers.MockConfServ(), client)) 
            {
                var employee = await hqClient.GetEmployee(123);
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Equal(123, employee.ID);
                Assert.Equal("Jan Kow", employee.Name);
                Assert.Equal("jank@gmail.com", employee.Email);
                Assert.Equal("1996-01-01", employee.DateOfBirth);
                Assert.Equal(2, employee.BranchOfficeID);
                Assert.Equal(30.0, employee.Pay);
                Assert.False(employee.IsManager);
            }
        }

        [Fact]
        public async Task ListEmployees_ShouldReturnEmptyListOfEmployees_WhenNoEmployees()
        {
            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(CommonHelpers.baseUrl + "/api/employees/list/*")
                    .Respond("application/json",
                    "[]"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(CommonHelpers.MockConfServ(), client)) 
            {
                var employees = await hqClient.ListEmployees(100);
                Assert.NotNull(employees);
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Empty(employees);
            }
        }

        /// <summary>
        /// Test that API server returns a list of employees
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonListOfEmployees()
        {
            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(CommonHelpers.baseUrl + "/api/employees/list/*")
                    .Respond("application/json",
                    "["+
                    "{'employee_id': 1, 'name' : 'Jan Kow', 'email': 'jank@gmail.com', 'date_of_birth': '1996-01-01', "+
                    " 'isManager': false, 'pay': 30.0,  'branch_office_id': 2 }, "+
                    "{'employee_id': 123, 'name' : 'Jan Kow2', 'email': 'jank2@gmail.com', 'date_of_birth': '1996-01-01', "+
                    " 'isManager': true, 'pay': 30.0,  'branch_office_id': 1 }, "+
                    "]"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(CommonHelpers.MockConfServ(), client)) 
            {
                var employees = await hqClient.ListEmployees(100);
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Equal(2, employees.Count);
                Assert.Equal(1, employees[0].ID);
                Assert.Equal(123, employees[1].ID);
                Assert.False(employees[0].IsManager);
                Assert.True(employees[1].IsManager);
            }
        }

        [Fact]
        public async Task ListSalariesForEmployee_ShouldNotThrowException()
        {
            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(CommonHelpers.baseUrl + "/api/salaries/list/*")
                    .Respond(HttpStatusCode.NotFound);

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(CommonHelpers.MockConfServ(), client)) 
            {
                var salaries = await hqClient.ListSalariesForEmployee(1);
                Assert.Empty(salaries);
            }
        }

        [Fact]
        public async Task ListSalariesForEmployee_ShouldReturnEmptyListOfSalaries_WhenNoSalaries()
        {
            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(CommonHelpers.baseUrl + "/api/salaries/list/*")
                    .Respond("application/json",
                    "[]"); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(CommonHelpers.MockConfServ(), client)) 
            {
                var salaries = await hqClient.ListSalariesForEmployee(1);
                // GetMatchCount will return the number of times a mocked request (returned by When / Expect) was called
                // https://github.com/richardszalay/mockhttp#verifying-matches
                Assert.Equal(1, mockHttp.GetMatchCount(mockedRequest));
                Assert.Empty(salaries);
            }
        }

        /// <summary>
        /// Test that API server returns a list of salaries
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ShouldReturnJsonListOfSalaries()
        {
            // https://github.com/richardszalay/mockhttp
            var mockHttp = new MockHttpMessageHandler();
            // Setup a respond for the user api (including a wildcard in the URL)
            var mockedRequest = mockHttp.When(CommonHelpers.baseUrl + "/api/salaries/list/*")
                    .Respond("application/json",
                    "[ {'id' : 98, 'timePeriod': '2019-Jun-15_2019-Jun-21', 'value': 300, 'employeeId': 1 }," +
                    "{'id' : 99, 'timePeriod': '2019-Jun-22_2019-Jun-28', 'value': 320, 'employeeId': 1}" +
                    " ] "); // Respond with JSON

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();
            using (var hqClient = new HQAPIClient(CommonHelpers.MockConfServ(), client)) 
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
