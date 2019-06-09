using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
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
                    Value = 15, TimePeriod = "20.1.2019-26.01.2019", EmployeeId = 1, HoursCount = 100, EmployeeHoursId = 1});
                employeeHours.Add(new EmployeeHours { 
                    Value = 10, TimePeriod = "27.01.2019-02.02.2019", EmployeeId = 1, HoursCount = 24, EmployeeHoursId = 2 });
                employeeHours.Add(new EmployeeHours { 
                    Value = 12, TimePeriod = "03.02.2019-09.02.2019", EmployeeId = 1, HoursCount = 66, EmployeeHoursId = 3 });
                employeeHours.Add(new EmployeeHours { 
                    Value = 2, TimePeriod = "20.1.2019-26.01.2019", EmployeeId = 2, HoursCount = 100, EmployeeHoursId = 4 });

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
            ConfigureLogging();
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

        public static void ConfigureLogging() {
            string log4net = "log4net.xml";
            if(File.Exists(log4net)) 
                XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new System.IO.FileInfo(log4net));
            else
                Console.WriteLine("Failed to configure log4net. log4net.xml does not exist in {0}", Environment.CurrentDirectory);
        }
    }
}
