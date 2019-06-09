using System.Threading.Tasks;

namespace BranchOfficeBackend
{
    public interface IUserRepository
    {
        Task<bool> IsValidAsync(string username, string password);

        Task<bool> IsManager(string username);

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="username"></param>
        /// <returns>the password in plain text</returns>
        Task<string> CreateUser(string username);
        Task DeleteUser(string username);
    }
}