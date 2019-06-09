using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Carter;
using Carter.ModelBinding;
using Carter.Request;
using Carter.Response;

namespace BranchOfficeBackend
{
    public class UserModule : CarterModule {
        public UserModule(IUserRepository userRepository) {
            this.Post("/api/user/{username}",  async(req, res, routeData) => {
                string username = routeData.As<string>("username");
                try {
                    string pass = await userRepository.CreateUser(username);
                    await res.Negotiate(new UserCreatedResponse() {
                        Password = pass,
                        Message = $"User {username} was created"
                        // TODO: Type 
                    });
                }
                catch(EmployeeNotFoundException notFound) {
                    res.StatusCode = 404;
                    await res.Negotiate(new UserCreatedResponse() {
                        Message = $"User {username} was not found.\n" + notFound.ToString()
                        // TODO: Type 
                    });
                }
                catch(UserAlreadyExistsException) {
                    res.StatusCode = 409;
                    await res.Negotiate(new UserCreatedResponse() {
                        Message = $"User {username} is already registered."
                        // TODO: Type 
                    });
                }
            });
            
            this.Delete("/api/user/{username}",  async(req, res, routeData) => {
                string username = routeData.As<string>("username");
                try {
                    var context = req.HttpContext;
                    var authenticated = context?.User?.Identity != null && context.User.Identity.IsAuthenticated;
                    if (!authenticated)
                    {
                        context.Response.StatusCode = 401;
                        await res.Negotiate(new ApiResponse() { Message = "Authentication required" });
                        return;
                    }

                    if(!IsManager(context.User.Claims)) {
                        context.Response.StatusCode = 403;
                        await res.Negotiate(new ApiResponse() { Message = "Unauthorized. Only manager can delete users" });
                        return;
                    }

                    await userRepository.DeleteUser(username);
                }
                catch(EmployeeNotFoundException notFound) {
                    res.StatusCode = 404;
                    await res.Negotiate(new UserCreatedResponse() {
                        Message = $"User {username} was not found.\n" + notFound.ToString()
                        // TODO: Type 
                    });
                }
                catch(UserAlreadyExistsException) {
                    res.StatusCode = 409;
                    await res.Negotiate(new UserCreatedResponse() {
                        Message = $"User {username} is already registered."
                        // TODO: Type 
                    });
                }
            });
        }

        private bool IsManager(IEnumerable<Claim> claims)
        {
            Claim role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if(role != null && role.Value == "Manager")
                return true;
            return false;
        }
    }
}