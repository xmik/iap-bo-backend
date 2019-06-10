using Microsoft.EntityFrameworkCore.Migrations;

namespace BranchOfficeBackend.Migrations
{
    public partial class RemoveDupHoursColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoursCount",
                table: "EmployeeHoursCollection");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HoursCount",
                table: "EmployeeHoursCollection",
                nullable: false,
                defaultValue: 0);
        }
    }
}
