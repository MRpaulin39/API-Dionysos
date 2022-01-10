using Microsoft.EntityFrameworkCore.Migrations;

namespace DIONYSOS.API.Data.Migrations
{
    public partial class DionysosDBV13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Customers",
                newName: "ZipCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "Customers",
                newName: "Code");
        }
    }
}
