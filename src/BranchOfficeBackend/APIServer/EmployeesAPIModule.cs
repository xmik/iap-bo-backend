using Carter;
using Carter.ModelBinding;
using Carter.Request;
using Carter.Response;

namespace BranchOfficeBackend
{
    // read also: https://stormpath.com/blog/routing-in-asp-net-core
    // for other routing building solutions
    public class EmployeesAPIModule : CarterModule
    {
        public EmployeesAPIModule(IEmployeeRepository service)
        {
            Get("/employee/list", async(req, res, routeData) => {
                var list = service.GetAllEmployees();
                // generates suitable response, by default in JSON
                await res.Negotiate(list);
            });

        }
    }
}
