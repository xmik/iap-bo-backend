using System;
using System.Net.Http.Headers;

namespace BranchOfficeBackend.Tests
{
    public static class CommonHelpers
    {
        public static string baseUrl = "http://localhost:1234";
        public static IConfigurationService MockConfServ(bool startSynchro=false){
            var cs = new Moq.Mock<IConfigurationService>();
            cs.Setup(m => m.GetHQServerUrl()).Returns(baseUrl);
            cs.Setup(m => m.GetStartSynchronizationLoopWithHQ()).Returns(startSynchro);
            return cs.Object;
        }

        public const string DefaultUsername = "ewa@example.com";
        public const string DefaultPassword = "pass";


        public static void AddBasicAuthHeader(this System.Net.Http.HttpClient client, string username = DefaultUsername, string password = DefaultPassword)
        {
            client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue(
                                    "Basic", Convert.ToBase64String(
                                        System.Text.ASCIIEncoding.ASCII.GetBytes(
                                        $"{DefaultUsername}:{DefaultPassword}")));
        }
    }
}