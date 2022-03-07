using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIONYSOS.API.Data.Migrations
{
    public partial class DionysosDBV211 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "APIUser",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "AuthUser",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "APIUser",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "AuthUser");
        }
    }
}
