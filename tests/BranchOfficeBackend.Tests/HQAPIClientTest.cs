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
        public async Task ShouldReturnJsonOneBranchOffices()
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
    }
}