using System;

namespace BranchOfficeBackend
{
    class Program
    {
        static void Main(string[] args)
        {
          Console.WriteLine("hello");
            // // manually add some data into database
            // Console.WriteLine("Updating database!");
            // using (var db = new EmployeeContext())
            // {
            //     db.Projects.Add(new Project { Name = "Economics" });
            //     db.Projects.Add(new Project { Name = "Law" });
            //     db.Projects.Add(new Project { Name = "Management" });
            //     var count = db.SaveChanges();
            //     Console.WriteLine("{0} Project records saved to database", count);
            //
            //     db.Employees.Add(new Employee { Name = "Jan Kowalski", Email = "jan@gmail.com" });
            //     db.Employees.Add(new Employee { Name = "Krzysztof Nowak", Email = "krzy@gmail.com" });
            //     var countEmployees = db.SaveChanges();
            //     Console.WriteLine("{0} Employee records saved to database", countEmployees);
            //
            //     Console.WriteLine();
            //     Console.WriteLine("Finished database update");
            //     Console.WriteLine("All Employees in database:");
            //     foreach (var employee in db.Employees)
            //     {
            //         Console.WriteLine(employee);
            //     }
            //     Console.WriteLine("All Projects in database:");
            //     foreach (var project in db.Projects)
            //     {
            //         Console.WriteLine(project);
            //     }
            //}

            // run API SERVER here
        }
    }
}
