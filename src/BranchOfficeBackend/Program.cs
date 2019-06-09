using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace BranchOfficeBackend
{
    class Program
    {
        private static CancellationTokenSource cts;

        /// <summary>
        /// Generate test data and save into database
        /// </summary>
        private static void GenerateTestData(){
            List<Employee> employees = new List<Employee>();
                employees.Add(new Employee { Name = "Jan Kowalski", Email = "jan@gmail.com" });
                employees.Add(new Employee { Name = "Krzysztof Nowak", Email = "krzy@gmail.com" });
                employees.Add(new Employee { Name = "Ala Jeden", Email = "ala1@gmail.com" });
                employees.Add(new Employee { Name = "Ola Dwa", Email = "ola2@gmail.com" });

            List<EmployeeHours> employeeHours = new List<EmployeeHours>();
                employeeHours.Add(new EmployeeHours { 
                    Value = 15, TimePeriod = "2019-1-20_2019-1-26", EmployeeId = 0});
                employeeHours.Add(new EmployeeHours { 
                    Value = 10, TimePeriod = "2019-1-27_2019-2-2", EmployeeId = 0 });
                employeeHours.Add(new EmployeeHours { 
                    Value = 12, TimePeriod = "2019-2-3_2019-2-9", EmployeeId = 0 });
                employeeHours.Add(new EmployeeHours { 
                    Value = 2, TimePeriod = "2019-1-20_2019-1-26", EmployeeId = 2 });

            Console.WriteLine("Updating database!");

            using (var db = new BranchOfficeDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.Migrate();

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
                Console.WriteLine("All EmployeeHours in database:");
                foreach (var eh in db.EmployeeHoursCollection)
                {
                    Console.WriteLine(eh);
                }
                Console.WriteLine();
            }
        }

        static void RunAPIServer()
        {
            // http://www.codedigest.com/quick-start/5/learn-kestrel-webserver-in-10-minutes
            var host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureWebHostBuilder()
                .Build();

            host.Run();
        }        

        static void Main(string[] args)
        {
            cts = new CancellationTokenSource();
            Console.CancelKeyPress += Console_CancelKeyPress;

            // TODO: run this only if testing. In production, synchronize wih HeadQuarters.
            GenerateTestData();

            RunAPIServer();

            Console.WriteLine("Good bye!");
        }

        private static void Console_CancelKeyPress (object sender, ConsoleCancelEventArgs e)
		{
			if (!cts.IsCancellationRequested) {
				cts.Cancel ();
				e.Cancel = true;
			}
		}
    }
}
