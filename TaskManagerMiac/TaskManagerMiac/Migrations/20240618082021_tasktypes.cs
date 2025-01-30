using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class tasktypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "task_state",
                keyColumn: "id_task_state",
                keyValue: 5,
                column: "title",
                value: "Ожидает отклонения руководителя");

            migrationBuilder.InsertData(
                table: "task_state",
                columns: new[] { "id_task_state", "title" },
                values: new object[] { 6, "Ожидает одобрения руководителя" });

            migrationBuilder.InsertData(
                table: "task_type",
                columns: new[] { "id_task_type", "title" },
                values: new object[,]
                {
                    { 1, "Финансовая" },
                    { 2, "Обычная" },
                    { 4, "Персональная" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "task_state",
                keyColumn: "id_task_state",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "task_type",
                keyColumn: "id_task_type",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "task_type",
                keyColumn: "id_task_type",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "task_type",
                keyColumn: "id_task_type",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "task_state",
                keyColumn: "id_task_state",
                keyValue: 5,
                column: "title",
                value: "Отменена пользователем");
        }
    }
}
