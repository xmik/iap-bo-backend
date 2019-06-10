﻿// <auto-generated />
using System;
using BranchOfficeBackend;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BranchOfficeBackend.Migrations
{
    [DbContext(typeof(BranchOfficeDbContext))]
    partial class BranchOfficeDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("BranchOfficeBackend.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("Email");

                    b.Property<bool>("IsManager");

                    b.Property<string>("Name");

                    b.HasKey("EmployeeId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("BranchOfficeBackend.EmployeeHours", b =>
                {
                    b.Property<int>("EmployeeHoursId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EmployeeId");

                    b.Property<int>("HoursCount");

                    b.Property<string>("TimePeriod");

                    b.Property<float>("Value");

                    b.HasKey("EmployeeHoursId");

                    b.ToTable("EmployeeHoursCollection");
                });

            modelBuilder.Entity("BranchOfficeBackend.Salary", b =>
                {
                    b.Property<int>("SalaryId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EmployeeId");

                    b.Property<string>("TimePeriod");

                    b.Property<float>("Value");

                    b.HasKey("SalaryId");

                    b.ToTable("Salaries");
                });
#pragma warning restore 612, 618
        }
    }
}
