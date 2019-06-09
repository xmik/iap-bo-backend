using System;
using Carter;
using Carter.ModelBinding;
using Carter.Request;
using Carter.Response;

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
        }
    }
}