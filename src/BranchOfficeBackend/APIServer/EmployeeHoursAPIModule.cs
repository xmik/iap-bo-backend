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
        public EmployeeHoursAPIModule(IWebObjectService service)
        {
            Get("/api/employee_hours/list/{employeeId:int}", async(req, res, routeData) => {
                int id = routeData.As<int>("employeeId");
                var list = service.GetAllEmployeeHours(id);
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
                var result = req.BindAndValidate<WebEmployeeHours>();

                try {
                    service.AddEmployeeHours(result.Data);
                } catch (ArgumentException ex) {
                    res.StatusCode = 400;
                    await res.WriteAsync(String.Format("Problem when adding the object to database: {0}", ex.Message));
                    return;
                }
                
                res.StatusCode = 201;
                await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                return;
            });
        }
    }
}