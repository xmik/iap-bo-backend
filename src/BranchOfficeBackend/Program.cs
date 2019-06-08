using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace BranchOfficeBackend
{
    class Program
    {
        private static CancellationTokenSource cts;
        private static HQAPIClient hqApiClient;
        private static int branchOfficeId;

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
                employeeHours.Add(new EmployeeHours { Value = 15, TimePeriod = new TimeSpan(), Employee = employees[0]});
                employeeHours.Add(new EmployeeHours { Value = 10, TimePeriod = new TimeSpan(), Employee = employees[0] });
                employeeHours.Add(new EmployeeHours { Value = 12, TimePeriod = new TimeSpan(), Employee = employees[0] });
                employeeHours.Add(new EmployeeHours { Value = 2, TimePeriod = new TimeSpan(), Employee = employees[2] });

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

        static async void SynchronizeWithHQServer()
        {
            if (hqApiClient != null)
            {
                try {
                    List<HQEmployee> employees = await hqApiClient.ListEmployees(branchOfficeId);
                    // TODO; replace bo db contents with the above employees
                    for (int i=0; i< employees.Count; i++)
                    {
                        List<HQSalary> salaries = await hqApiClient.ListSalariesForEmployee(i);
                        // TODO; replace bo db contents with the above salaries
                    }
                } catch (System.Net.Http.HttpRequestException ex) {
                    Console.WriteLine("Synchronizing with HQ failed (is the HQ server running?)");
                }
            }
        }

        static void SynchronizeWithHQServerLoop()
        {
            int frequency_seconds = 5;
            while(true)
            {   
                if (cts.Token.IsCancellationRequested) 
                {
                    Console.WriteLine("Ctrl+C caught when synchronizing with HQ, exit");
                    return;
                }
                Console.WriteLine("Synchronizing with HQ (every {0} seconds)", frequency_seconds.ToString());
                SynchronizeWithHQServer();
                Thread.Sleep(frequency_seconds*1000);
            }
        }

        static void Main(string[] args)
        {
            cts = new CancellationTokenSource();
            Console.CancelKeyPress += Console_CancelKeyPress;
            branchOfficeId = 0;

            // TODO: run this only if testing. In production, synchronize wih HeadQuarters.
            GenerateTestData();

            // run BO Api Server in the background (in another thread),
            // so that it can be running before we synchronize with HQ Api Server
            // and so HQ Server does not have to wait for Branch Office Server
            Thread boApiServerThread = new Thread (RunAPIServer);
            boApiServerThread.Start();

            string hqApiUrl = "http://localhost:8000";
            hqApiClient = new HQAPIClient(hqApiUrl);

            // run Synchronization with HQ Api Server in the background (in another thread)
            Thread hqApiClientThread = new Thread (SynchronizeWithHQServerLoop);
            hqApiClientThread.Start();

            hqApiClientThread.Join();
            boApiServerThread.Interrupt();
            boApiServerThread.Join();
            if (hqApiClient != null) {
                hqApiClient.Dispose();
            }
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
