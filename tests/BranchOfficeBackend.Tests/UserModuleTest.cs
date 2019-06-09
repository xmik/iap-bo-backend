using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using BranchOfficeBackend;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace BranchOfficeBackend.Tests
{
    /// <summary>
    /// Unit tests the API server class responsible for user and authentication management
    /// </summary>
    public class UserModuleTest
    {
        [Fact]
        public async Task ShouldCreateNewUserInRepository()
        {
            var mock = new Moq.Mock<IUserRepository>();
            mock.Setup(m => m.CreateUser(It.IsAny<string>())).ReturnsAsync("pass");
            using(var testServer = new TestServerBuilder()
                .WithMock<IUserRepository>(typeof(IUserRepository), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.PostAsync("/api/user/ewa@example.com", new StringContent(""));
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);    
                var jsonString = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<UserCreatedResponse>(jsonString);
                Assert.Equal("pass", items.Password);                            
            }
        }

        [Fact]
        public async Task PostNewUserShouldReturn404WhenEmailNotFound()
        {
            var mock = new Moq.Mock<IUserRepository>();
            mock.Setup(m => m.CreateUser(It.IsAny<string>())).ThrowsAsync(new EmployeeNotFoundException());
            using(var testServer = new TestServerBuilder()
                .WithMock<IUserRepository>(typeof(IUserRepository), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.PostAsync("/api/user/ewa@example.com", new StringContent(""));
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task PostNewUserShouldReturn409WhenAlreadyExists()
        {
            var mock = new Moq.Mock<IUserRepository>();
            mock.Setup(m => m.CreateUser(It.IsAny<string>())).ThrowsAsync(new UserAlreadyExistsException());
            using(var testServer = new TestServerBuilder()
                .WithMock<IUserRepository>(typeof(IUserRepository), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.PostAsync("/api/user/ewa@example.com", new StringContent(""));
                Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
            }
        }

        [Fact]
        public async Task ShouldDeleteUserInRepositoryWhenRequestedByManager()
        {
            var mock = new Moq.Mock<IUserRepository>();
            mock.Setup(m => m.DeleteUser(It.IsAny<string>())).Returns(Task.CompletedTask);
            mock.Setup(m => m.IsValidAsync("ewa@example.com", "pass")).ReturnsAsync(true);
            mock.Setup(m => m.IsManager("ewa@example.com")).ReturnsAsync(true);
            using(var testServer = new TestServerBuilder()
                .WithMock<IUserRepository>(typeof(IUserRepository), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                client.AddBasicAuthHeader("ewa@example.com", "pass");
                var response = await client.DeleteAsync("/api/user/ewa@example.com");
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task DeleteUserShouldRespond401WhenNoAuth()
        {
            var mock = new Moq.Mock<IUserRepository>();
            using(var testServer = new TestServerBuilder()
                .WithMock<IUserRepository>(typeof(IUserRepository), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                var response = await client.DeleteAsync("/api/user/ewa@example.com");
                Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task DeleteUserShouldRespond403WhenNotAManager()
        {
            var mock = new Moq.Mock<IUserRepository>();
            mock.Setup(m => m.DeleteUser(It.IsAny<string>())).Returns(Task.CompletedTask);
            mock.Setup(m => m.IsValidAsync("ewa@example.com", "pass")).ReturnsAsync(true);
            mock.Setup(m => m.IsManager("ewa@example.com")).ReturnsAsync(false);
            using(var testServer = new TestServerBuilder()
                .WithMock<IUserRepository>(typeof(IUserRepository), mock)
                .Build())
            {
                var client = testServer.CreateClient();
                client.AddBasicAuthHeader("ewa@example.com", "pass");
                var response = await client.DeleteAsync("/api/user/ewa@example.com");
                Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
            }
        }
    }
}