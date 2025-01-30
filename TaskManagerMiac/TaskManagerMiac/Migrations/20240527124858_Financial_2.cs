using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class Financial_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Guarantee",
                table: "financial_task_document",
                newName: "guarantee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "guarantee",
                table: "financial_task_document",
                newName: "Guarantee");
        }
    }
}
