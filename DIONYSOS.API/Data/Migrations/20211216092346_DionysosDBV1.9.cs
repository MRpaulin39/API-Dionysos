using Microsoft.EntityFrameworkCore.Migrations;

namespace DIONYSOS.API.Data.Migrations
{
    public partial class DionysosDBV19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayOrderAuto",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "OrderAutoActivate",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOrderAuto",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "OrderAutoActivate",
                table: "Product");
        }
    }
}
