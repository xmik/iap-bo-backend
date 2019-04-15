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
        // TODO: maybe make it static https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client#create-and-initialize-httpclient
        private HttpClient _client;
        private string _baseUrl;

        // https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        public HQAPIClient(string baseUrl, HttpClient httpClient)
        {
            _baseUrl = baseUrl;
            _client = httpClient;
            _client.BaseAddress = new Uri(baseUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public HQAPIClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseUrl);
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
            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(_baseUrl != null ? _baseUrl.TrimEnd('/') : "").Append(requestUrl);
            return urlBuilder.ToString();
        }

        // http://blog.stephencleary.com/2012/02/async-and-await.html
        public async Task<HQBranchOffice> GetBranchOffice(int branchOfficeId)
        {
            string url = this.BuildUrl("/api/branch_offices/" + branchOfficeId);
            string json;
            // get response json
            // TODO: maybe extract this code to a method
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
            // deserialize the json response into C# objects
            HQBranchOffice result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                HQBranchOffice>(json);
            return result;  
        }

        // http://blog.stephencleary.com/2012/02/async-and-await.html
        public async Task<List<HQBranchOffice>> ListBranchOffices()
        {
            string url = this.BuildUrl("/api/branch_offices");
            string json;
            // get response json
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
            // deserialize the json response into C# objects
            Dictionary<string, List<HQBranchOffice>> result = Newtonsoft.Json.JsonConvert.DeserializeObject<
                    Dictionary<string, List<HQBranchOffice>>>(json);
            return result["branch_offices"];            
        }

    }
}
