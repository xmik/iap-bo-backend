using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bazinga.AspNetCore.Authentication.Basic;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BranchOfficeBackend
{
    public class MyAssemblyCatalog : DependencyContextAssemblyCatalog {
        public override IReadOnlyCollection<Assembly> GetAssemblies() {
            var assemblies = new [] { typeof(EmployeesAPIModule).Assembly };
            return assemblies;
        }
    }

    // the Startup class defines the services and middleware pipeline for an ASP.NET Core application
    // https://www.stevejgordon.co.uk/aspnet-core-anatomy-how-does-usestartup-work
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public static readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; private set; }

        public static void ConfigureServices(IServiceCollection services)
        {
            // Add services to the collection. Don't build or return
            // any IServiceProvider or the ConfigureContainer method
            // won't get called.

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:8000",
                                        "https://localhost:8000",
                                        "http://bo:8000",
                                        "https://bo:8000")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
            });

            // This registration of assembly with carter modules is necessary for tests to work.
            // see https://github.com/CarterCommunity/Carter/pull/88
            services.AddCarter(new MyAssemblyCatalog());
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you. If you
        // need a reference to the container, you need to use the
        // "Without ConfigureContainer" mechanism shown later.
        public static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new BranchOfficeAutofacModule());
        }

        // Configure is where you add middleware. This is called after
        // ConfigureContainer. You can use IApplicationBuilder.ApplicationServices
        // here if you need to resolve things from the container.
        public static void Configure(IApplicationBuilder app)
        {
            app.UseCors(MyAllowSpecificOrigins); 
            app.UseCarter(new CarterOptions(ctx => AuthenticateBeforeHook(ctx, 
                app.ApplicationServices.GetService<IUserRepository>())));
        }

        // https://tools.ietf.org/html/rfc2617#section-2
        private static (string userid, string password) DecodeUserIdAndPassword(string encodedAuth)
        {
            var userpass = Encoding.UTF8.GetString(Convert.FromBase64String(encodedAuth));

            var separator = userpass.IndexOf(':');
            if (separator == -1) throw new InvalidOperationException("Invalid Authorization header: Missing separator character ':'. See RFC2617.");

            return (userpass.Substring(0, separator), userpass.Substring(separator + 1));
        }

        private static async Task<bool> AuthenticateBeforeHook(HttpContext context, IUserRepository userRepository)
        {
            string auth = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(auth))
            {
                return true;
            }

            string encodedAuth = null;
            if (auth.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                encodedAuth = auth.Substring("Basic ".Length).Trim();
            }

            if (string.IsNullOrEmpty(encodedAuth))
            {
                return true;
            }

            var userpass = DecodeUserIdAndPassword(encodedAuth);

            if (!await userRepository.IsValidAsync(userpass.userid, userpass.password))
            {
                return true;
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, userpass.userid, ClaimValueTypes.String));

            if (await userRepository.IsManager(userpass.userid)) {
                claims.Add(new Claim(ClaimTypes.Role, "Manager"));
            }
            else {
                claims.Add(new Claim(ClaimTypes.Role, "Non-Manager"));
            }
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));

            context.User = principal;
            return true;
        }
    }
}
