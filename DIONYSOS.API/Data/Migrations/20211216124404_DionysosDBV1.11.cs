using Microsoft.EntityFrameworkCore.Migrations;

namespace DIONYSOS.API.Data.Migrations
{
    public partial class DionysosDBV111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Receive",
                table: "OrderSupplier",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Receive",
                table: "OrderSupplier");
        }
    }
}
