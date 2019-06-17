using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BranchOfficeBackend
{
    public class HQAPIClient : IHQAPIClient, IDisposable
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(HQAPIClient)); 
        private HttpClient _client;
        private IConfigurationService _confServ;
        private string _baseUrl;

        // https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        public HQAPIClient(IConfigurationService confServ, HttpClient httpClient)
        {
            _confServ = confServ;
            _client = httpClient;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public HQAPIClient(IConfigurationService confServ)
        {
            _confServ = confServ;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }

        public string BuildUrl(string requestUrl)
        {
            if (_baseUrl == null) {
                _baseUrl = _confServ.GetHQServerUrl();
                _client.BaseAddress = new Uri(_baseUrl);
            }
            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(_baseUrl != null ? _baseUrl.TrimEnd('/') : "").Append(requestUrl);
            return urlBuilder.ToString();
        }

        private async Task<HttpResponseMessage> commonRequestOperation(string url) {
            HttpResponseMessage response;
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                // already set above:
                // request_.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                request_.RequestUri = new System.Uri(url, System.UriKind.RelativeOrAbsolute);

                response = await _client.SendAsync(request_);
            }
            return response;
        }

        // http://blog.stephencleary.com/2012/02/async-and-await.html
        public async Task<HQBranchOffice> GetBranchOffice(int branchOfficeId)
        {
            string url = this.BuildUrl("/api/branch_offices/" + branchOfficeId);
            var response = await commonRequestOperation(url);
            if (response.StatusCode == HttpStatusCode.NotFound) {
                return null;
            }
            string json = await response.Content.ReadAsStringAsync();
            // deserialize the json response into C# objects
            HQBranchOffice result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                HQBranchOffice>(json);
            return result;  
        }

        // http://blog.stephencleary.com/2012/02/async-and-await.html
        public async Task<List<HQBranchOffice>> ListBranchOffices()
        {
            string url = this.BuildUrl("/api/branch_offices");
            var response = await commonRequestOperation(url);
            if (response.StatusCode == HttpStatusCode.NotFound) {
                return new List<HQBranchOffice>();
            }
            string json = await response.Content.ReadAsStringAsync();
            if (json == "[]")
                return new List<HQBranchOffice>();
            // deserialize the json response into C# objects
            Dictionary<string, List<HQBranchOffice>> result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                    Dictionary<string, List<HQBranchOffice>>>(json);
            return result["branch_offices"];            
        }

        public async Task<HQEmployee> GetEmployee(int employeeId)
        {
            string url = this.BuildUrl("/api/employees/" + employeeId);
            var response = await commonRequestOperation(url);
            if (response.StatusCode == HttpStatusCode.NotFound) {
                return null;
            }
            string json = await response.Content.ReadAsStringAsync();
            // deserialize the json response into C# objects
            HQEmployee result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                HQEmployee>(json);
            return result;  
        }

        public async Task<List<HQEmployee>> ListEmployees(int branchOfficeId)
        {
            string url = this.BuildUrl("/api/employees/list/" + branchOfficeId);
            var response = await commonRequestOperation(url);
            if (response.StatusCode == HttpStatusCode.NotFound) {
                return new List<HQEmployee>();
            }
            string json = await response.Content.ReadAsStringAsync();
            _log.DebugFormat("ListEmployees({0}) returned: {1}", branchOfficeId.ToString(), json);
            if (json == "[]")
                return new List<HQEmployee>();
            // deserialize the json response into C# objects
            List<HQEmployee> result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                    List<HQEmployee>>(json);
            return result;            
        }

        public async Task<List<HQSalary>> ListSalariesForEmployee(int employeeId)
        {
            string url = this.BuildUrl("/api/salaries/list/"+ employeeId);
            var response = await commonRequestOperation(url);
            if (response.StatusCode == HttpStatusCode.NotFound) {
                return new List<HQSalary>();
            }
            string json = await response.Content.ReadAsStringAsync();
            _log.DebugFormat("ListSalariesForEmployee({0}) returned: {1}", employeeId.ToString(), json);
            if (json == "[]")
                return new List<HQSalary>();
            // deserialize the json response into C# objects
            List<HQSalary> result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                    List<HQSalary>>(json);
            return result;            
        }
    }
}
