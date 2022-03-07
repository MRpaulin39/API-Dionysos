using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIONYSOS.API.Data.Migrations
{
    public partial class DionysosDBV21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "APIUser",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "APIUser");
        }
    }
}
