using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BranchOfficeBackend
{
    public class HQAPIClient : IHQAPIClient, IDisposable
    {
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

        private async Task<string> commonRequestOperation(string url) {
            string json;
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                // already set above:
                // request_.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                request_.RequestUri = new System.Uri(url, System.UriKind.RelativeOrAbsolute);

                var response = await _client.SendAsync(request_);
                response.EnsureSuccessStatusCode();
                json = await response.Content.ReadAsStringAsync();
            }
            return json;
        }

        // http://blog.stephencleary.com/2012/02/async-and-await.html
        public async Task<HQBranchOffice> GetBranchOffice(int branchOfficeId)
        {
            string url = this.BuildUrl("/api/branch_offices/" + branchOfficeId);
            string json = await commonRequestOperation(url);
            // deserialize the json response into C# objects
            HQBranchOffice result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                HQBranchOffice>(json);
            return result;  
        }

        // http://blog.stephencleary.com/2012/02/async-and-await.html
        public async Task<List<HQBranchOffice>> ListBranchOffices()
        {
            string url = this.BuildUrl("/api/branch_offices");
            string json = await commonRequestOperation(url);
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
            string json = await commonRequestOperation(url);
            // deserialize the json response into C# objects
            HQEmployee result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                HQEmployee>(json);
            return result;  
        }

        public async Task<List<HQEmployee>> ListEmployees(int branchOfficeId)
        {
            string url = this.BuildUrl("/api/employees/list/" + branchOfficeId);
            string json = await commonRequestOperation(url);
            if (json == "[]")
                return new List<HQEmployee>();
            // deserialize the json response into C# objects
            Dictionary<string, List<HQEmployee>> result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                    Dictionary<string, List<HQEmployee>>>(json);
            return result["employees"];            
        }

        public async Task<List<HQSalary>> ListSalariesForEmployee(int employeeId)
        {
            string url = this.BuildUrl("/api/salaries/list/"+ employeeId);
            string json = await commonRequestOperation(url);
            if (json == "[]")
                return new List<HQSalary>();
            // deserialize the json response into C# objects
            Dictionary<string, List<HQSalary>> result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                    Dictionary<string, List<HQSalary>>>(json);
            return result["salaries"];            
        }
    }
}
