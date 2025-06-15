using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinSync.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIncomeCategoryNavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Incomes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { 5, "Lazer" },
                    { 6, "Salário" },
                    { 7, "Investimentos" },
                    { 8, "Presentes" },
                    { 9, "Outros" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_CategoryId",
                table: "Incomes",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Categories_CategoryId",
                table: "Incomes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Categories_CategoryId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_CategoryId",
                table: "Incomes");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 9);

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Incomes");
        }
    }
}
