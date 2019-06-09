
using System.Threading.Tasks;
using Bazinga.AspNetCore.Authentication.Basic;

namespace BranchOfficeBackend
{
    public class DatabaseBasicCredentialVerifier : IBasicCredentialVerifier
    {
        private readonly IUserRepository _db;
        
        public DatabaseBasicCredentialVerifier() {} //IUserRepository db) => _db = db;

        public Task<bool> Authenticate(string username, string password)
        {
            return Task.FromResult(true);
            //return _db.IsValidAsync(username, password);
        }
    }
}