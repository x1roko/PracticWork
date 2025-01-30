using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class TestInititalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "group",
                columns: new[] { "id_group", "title" },
                values: new object[,]
                {
                    { 1, "IT-Отдел" },
                    { 2, "Не определён" },
                    { 3, "Отдел закупок" },
                    { 4, "Служба безопасности" },
                    { 5, "Юрист" }
                });

            migrationBuilder.InsertData(
                table: "priority",
                columns: new[] { "id_priority", "title", "weight" },
                values: new object[,]
                {
                    { 1, "Низкий", 0 },
                    { 2, "Средний", 50 },
                    { 3, "Высокий", 100 },
                    { 4, "Максимальный", 1000 }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "id_role", "title" },
                values: new object[,]
                {
                    { 1, "root" },
                    { 2, "admin" },
                    { 3, "group_admin" },
                    { 4, "default" }
                });

            migrationBuilder.InsertData(
                table: "task_state",
                columns: new[] { "id_task_state", "title" },
                values: new object[,]
                {
                    { 1, "Создана" },
                    { 2, "В работе" },
                    { 3, "Одобрена" },
                    { 4, "Отклонена" },
                    { 5, "Отменена пользователем" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "priority",
                keyColumn: "id_priority",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "priority",
                keyColumn: "id_priority",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "priority",
                keyColumn: "id_priority",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "priority",
                keyColumn: "id_priority",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "id_role",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "id_role",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "id_role",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "id_role",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "task_state",
                keyColumn: "id_task_state",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "task_state",
                keyColumn: "id_task_state",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "task_state",
                keyColumn: "id_task_state",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "task_state",
                keyColumn: "id_task_state",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "task_state",
                keyColumn: "id_task_state",
                keyValue: 5);
        }
    }
}
