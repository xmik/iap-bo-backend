using System;
using Carter;
using Carter.ModelBinding;
using Carter.Request;
using Carter.Response;

namespace BranchOfficeBackend
{
    public class SalariesAPIModule : CarterModule
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(SalariesAPIModule)); 
        public SalariesAPIModule(IWebObjectService service)
        {
            Get("/api/salaries/list", async(req, res, routeData) => {
                _log.Debug("Received HTTP request: GET /api/salaries/list");
                var list = service.GetAllSalaries();
                res.StatusCode = 200;
                // generates suitable response, by default in JSON
                await res.Negotiate(list);
            });

            Get("/api/salaries/list/{employeeId:int}", async(req, res, routeData) => {
                int id = routeData.As<int>("employeeId");
                _log.Debug(String.Format("Received HTTP request: GET /api/salaries/list/{0}",id));
                var obj = service.GetSalariesForAnEmployee(id);
                if (obj == null) {
                    res.StatusCode = 404;
                } else {
                    res.StatusCode = 200;
                    // generates suitable response, by default in JSON
                    await res.Negotiate(obj);
                }
            });
        }
    }
}
