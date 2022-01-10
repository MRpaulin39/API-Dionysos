using Microsoft.EntityFrameworkCore.Migrations;

namespace DIONYSOS.API.Data.Migrations
{
    public partial class DionysosDBV12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAlcohol_Product_ProductId",
                table: "ProductAlcohol");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductAlcohol",
                table: "ProductAlcohol");

            migrationBuilder.RenameTable(
                name: "ProductAlcohol",
                newName: "Alcohol");

            migrationBuilder.RenameIndex(
                name: "IX_ProductAlcohol_ProductId",
                table: "Alcohol",
                newName: "IX_Alcohol_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Alcohol",
                table: "Alcohol",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Alcohol_Product_ProductId",
                table: "Alcohol",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alcohol_Product_ProductId",
                table: "Alcohol");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Alcohol",
                table: "Alcohol");

            migrationBuilder.RenameTable(
                name: "Alcohol",
                newName: "ProductAlcohol");

            migrationBuilder.RenameIndex(
                name: "IX_Alcohol_ProductId",
                table: "ProductAlcohol",
                newName: "IX_ProductAlcohol_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductAlcohol",
                table: "ProductAlcohol",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAlcohol_Product_ProductId",
                table: "ProductAlcohol",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
