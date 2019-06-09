using System.Threading.Tasks;

namespace BranchOfficeBackend {
    public class UserRepository : IUserRepository
    {
        public Task<string> CreateUser(string username)
        {
            //TODO impl
            return Task.FromResult(username);
        }

        public Task<bool> IsValidAsync(string username, string password)
        {
            //TODO impl
            return Task.FromResult(true);
        }
    }
}