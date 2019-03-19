using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace BranchOfficeBackend
{
    public class BranchOfficeDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }

        public DbSet<EmployeeHours> EmployeeHoursCollection { get; set; }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=db;Database=postgres;Username=postgres;Password=my_pw");
        }
    }

    
}
