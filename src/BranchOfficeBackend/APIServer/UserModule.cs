using System;
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
        }
    }
}