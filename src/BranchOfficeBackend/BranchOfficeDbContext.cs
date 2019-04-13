using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace BranchOfficeBackend
{
    public class BranchOfficeDbContext : DbContext
    {
        public BranchOfficeDbContext(DbContextOptions<BranchOfficeDbContext> options)
            : base(options)
        {

        }
        public BranchOfficeDbContext()
        {        }

        public DbSet<EmployeeHours> EmployeeHoursCollection { get; set; }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // do not use the postgresql database named: postgres, because we cannot delete the database that
            // we are connected to
            // https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL/issues/845
            optionsBuilder.UseNpgsql("Host=db;Database=mydb;Username=postgres;Password=my_pw");
        }
    }


}
