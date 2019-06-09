using System;
using Carter;
using Carter.ModelBinding;
using Carter.Request;
using Carter.Response;

namespace BranchOfficeBackend
{
    // read also: https://stormpath.com/blog/routing-in-asp-net-core
    // for other routing building solutions
    //
    // do not use attributes way (decorator pattern), because then
    // we'd have to implement 1 api endpoint in many places.
    // Here we implement it all for 1 api endpoint in 1 place.
    public class EmployeesAPIModule : CarterModule
    {
        public EmployeesAPIModule(IWebObjectService service)
        {
            Get("/api/employees/list", async(req, res, routeData) => {
                var list = service.GetAllEmployees();
                res.StatusCode = 200;
                // generates suitable response, by default in JSON
                await res.Negotiate(list);
            });

            Get("/api/employees/{id:int}", async(req, res, routeData) => {
                int id = routeData.As<int>("id");
                var emp = service.GetEmployee(id);
                if (emp == null) {
                    res.StatusCode = 404;
                } else {
                    res.StatusCode = 200;
                    // generates suitable response, by default in JSON
                    await res.Negotiate(emp);
                }
            });
        }
    }
}
