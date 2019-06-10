using System;
using Carter;
using Carter.ModelBinding;
using Carter.Request;
using Carter.Response;
using Microsoft.AspNetCore.Http;

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
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(EmployeesAPIModule)); 
        public EmployeesAPIModule(IWebObjectService service)
        {
            Get("/api/employees/list", async(req, res, routeData) => {
                _log.Debug("Received HTTP request: GET /api/employees/list");
                var list = service.GetAllEmployees();
                res.StatusCode = 200;
                // generates suitable response, by default in JSON
                await res.Negotiate(list);
            });

            Get("/api/employees/{id:int}", async(req, res, routeData) => {
                int id = routeData.As<int>("id");
                _log.Debug(String.Format("Received HTTP request: GET /api/employees/{0}",id));
                var emp = service.GetEmployee(id);
                if (emp == null) {
                    res.StatusCode = 404;
                } else {
                    res.StatusCode = 200;
                    // generates suitable response, by default in JSON
                    await res.Negotiate(emp);
                }
            });

            Post("/api/employees", async(req, res, routeData) => {
                _log.Debug("Received HTTP request: POST /api/employees");
                var result = req.Bind<WebEmployee>();
                _log.DebugFormat("Employee object parsed from user: {0}", result);

                try {
                    service.AddEmployee(result);
                } catch (ArgumentException ex) {
                    res.StatusCode = 400;
                    await res.WriteAsync(String.Format("Problem when adding the object to database: {0}", ex.Message));
                    return;
                }
                
                res.StatusCode = 201;
                await res.WriteAsync(String.Format("Employee was correctly added!"));
                return;
            });
        }
    }
}
