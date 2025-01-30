using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class initpaths3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "task_type_path",
                columns: new[] { "id_group", "id_task_type", "index" },
                values: new object[,]
                {
                    { 3, 1, 2 },
                    { 6, 1, 3 },
                    { 7, 1, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "task_type_path",
                keyColumns: new[] { "id_group", "id_task_type" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "task_type_path",
                keyColumns: new[] { "id_group", "id_task_type" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "task_type_path",
                keyColumns: new[] { "id_group", "id_task_type" },
                keyValues: new object[] { 7, 1 });
        }
    }
}
