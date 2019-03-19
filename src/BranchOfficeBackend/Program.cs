using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace BranchOfficeBackend
{
    class Program
    {
        /// <summary>
        /// Generate test data and save into database
        /// </summary>
        private static void GenerateTestData(){
            List<Project> projects = new List<Project>();
                projects.Add(new Project { Name = "Economics" });
                projects.Add(new Project { Name = "History" });
                
            List<Employee> employees = new List<Employee>();
                employees.Add(new Employee { Name = "Jan Kowalski", Email = "jan@gmail.com", Project =  projects[0] });
                employees.Add(new Employee { Name = "Krzysztof Nowak", Email = "krzy@gmail.com", Project =  projects[0] });
                employees.Add(new Employee { Name = "Ala Jeden", Email = "ala1@gmail.com", Project =  projects[1] });
                employees.Add(new Employee { Name = "Ola Dwa", Email = "ola2@gmail.com" });
                
            List<EmployeeHours> employeeHours = new List<EmployeeHours>();
                employeeHours.Add(new EmployeeHours { Value = 15, TimePeriod = new TimeSpan(), Employee = employees[0], Project = employees[0].Project });
                employeeHours.Add(new EmployeeHours { Value = 10, TimePeriod = new TimeSpan(), Employee = employees[0], Project = employees[0].Project });
                employeeHours.Add(new EmployeeHours { Value = 12, TimePeriod = new TimeSpan(), Employee = employees[0], Project = employees[0].Project });
                employeeHours.Add(new EmployeeHours { Value = 2, TimePeriod = new TimeSpan(), Employee = employees[2], Project = employees[2].Project });

            Console.WriteLine("Updating database!");
            
            using (var db = new BranchOfficeDbContext(null))
            {                
                // this will remove all rows from all tables, but newly added entities
                // will have new IDs, higher than IDs of those deleted rows
                db.Database.ExecuteSqlCommand("DELETE FROM \"EmployeeHoursCollection\"");
                db.Database.ExecuteSqlCommand("DELETE FROM \"Employees\"");
                db.Database.ExecuteSqlCommand("DELETE FROM \"Projects\"");

                db.Projects.AddRange(projects);
                var count = db.SaveChanges();
                Console.WriteLine("{0} Project records saved to database", count);            
                db.Employees.AddRange(employees);
                var countEmployees = db.SaveChanges();
                Console.WriteLine("{0} Employee records saved to database", countEmployees);            
                db.EmployeeHoursCollection.AddRange(employeeHours);
                var countEmployeeHours = db.SaveChanges();
                Console.WriteLine("{0} EmployeeHours records saved to database", countEmployeeHours);
            
                Console.WriteLine();
                Console.WriteLine("Finished database update");

                Console.WriteLine("All Employees in database:");
                foreach (var employee in db.Employees)
                {
                    Console.WriteLine(employee);
                }
                Console.WriteLine("All Projects in database:");
                foreach (var project in db.Projects)
                {
                    Console.WriteLine(project);
                }
                Console.WriteLine("All EmployeeHours in database:");
                foreach (var eh in db.EmployeeHoursCollection)
                {
                    Console.WriteLine(eh);
                }
            }
        }

        static void RunAPIServer()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureWebHostBuilder()
                .Build();

            host.Run();
        }

        static void Main(string[] args)
        {
            // TODO: run this only if testing. In production, synchronize wih HeadQuarters.
            GenerateTestData();

            // run API SERVER here
            RunAPIServer();
        }
    }
}
