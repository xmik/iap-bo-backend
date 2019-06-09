using System.IO;
using Microsoft.AspNetCore.Hosting;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Bazinga.AspNetCore.Authentication.Basic;
using Microsoft.Extensions.DependencyInjection;

namespace BranchOfficeBackend
{    
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureWebHostBuilder(this IWebHostBuilder builder) {
            return builder.UseContentRoot(Directory.GetCurrentDirectory())                
                .ConfigureServices(services => {
                    services.AddAutofac();
                    services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                        .AddBasicAuthentication<DatabaseBasicCredentialVerifier>();
                })
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:8080");
        }
    }
}