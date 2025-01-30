using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class FinanceUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "finance_source",
                table: "financial_task_document",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "kbk",
                table: "financial_task_document",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "kvr",
                table: "financial_task_document",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "law",
                table: "financial_task_document",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "finance_source",
                table: "financial_task_document");

            migrationBuilder.DropColumn(
                name: "kbk",
                table: "financial_task_document");

            migrationBuilder.DropColumn(
                name: "kvr",
                table: "financial_task_document");

            migrationBuilder.DropColumn(
                name: "law",
                table: "financial_task_document");
        }
    }
}
