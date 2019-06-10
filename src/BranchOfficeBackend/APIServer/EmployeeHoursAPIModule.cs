using System;
using Carter;
using Carter.ModelBinding;
using Carter.Request;
using Carter.Response;
using Microsoft.AspNetCore.Http;

namespace BranchOfficeBackend
{
    public class EmployeeHoursAPIModule: CarterModule
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(EmployeeHoursAPIModule)); 
        public EmployeeHoursAPIModule(IWebObjectService service)
        {
            Get("/api/employee_hours/list/{employeeId:int}", async(req, res, routeData) => {
                int id = routeData.As<int>("employeeId");
                _log.Debug(String.Format("Received HTTP request: GET /api/employee_hours/list/{0}",id));
                var list = service.GetEmployeeHoursForAnEmployee(id);
                if (list == null || list.Count == 0)
                {
                    res.StatusCode = 404;
                } else {
                    res.StatusCode = 200;
                    await res.Negotiate(list);
                }
            });

            Get("/api/employee_hours/{employeeHoursId:int}", async(req, res, routeData) => {
                int id = routeData.As<int>("employeeHoursId");
                _log.Debug(String.Format("Received HTTP request: GET /api/employee_hours/{0}",id));
                var weh = service.GetOneEmployeeHours(id);
                if (weh == null)
                {
                    res.StatusCode = 404;
                } else {
                    res.StatusCode = 200;
                    await res.Negotiate(weh);
                }
            });

            Post("/api/employee_hours", async(req, res, routeData) => {
                _log.Debug("Received HTTP request: POST /api/employee_hours");
                var result = req.Bind<WebEmployeeHours>();
                _log.DebugFormat("EmployeeHours object parsed from user: {0}", result);

                try {
                    service.AddEmployeeHours(result);
                } catch (ArgumentException ex) {
                    res.StatusCode = 400;
                    await res.WriteAsync(String.Format("Problem when adding the object to database: {0}", ex.Message));
                    return;
                }
                
                res.StatusCode = 201;
                await res.WriteAsync(String.Format("EmployeeHours was correctly added!"));
                return;
            });

            Delete("/api/employee_hours/{employeeHoursId:int}", async(req, res, routeData) => {
                int id = routeData.As<int>("employeeHoursId");
                _log.Debug(String.Format("Received HTTP request: DELETE /api/employee_hours/{0}",id));
                var weh = service.GetOneEmployeeHours(id);
                if (weh == null)
                {
                    res.StatusCode = 404;
                } else {
                    res.StatusCode = 202;
                    service.DeleteEmployeeHours(id);
                    await res.Negotiate(weh);
                }
            });

            Put("/api/employee_hours", async(req, res, routeData) => {
                _log.Debug("Received HTTP request: PUT /api/employee_hours");
                var result = req.BindAndValidate<WebEmployeeHours>();
                var weh = service.GetOneEmployeeHours(result.Data.Id);
                if (weh == null)
                {
                    res.StatusCode = 404;
                } else {
                    try {
                    service.EditEmployeeHours(result.Data);
                    } catch (ArgumentException ex) {
                        res.StatusCode = 400;
                        await res.WriteAsync(String.Format("Problem when editing the object in database: {0}", ex.Message));
                        return;
                    }
                    res.StatusCode = 202;
                    await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                }                
            });
        }
    }
}