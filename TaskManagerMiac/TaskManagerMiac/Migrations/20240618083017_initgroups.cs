using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class initgroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 1,
                column: "title",
                value: "Отдел Информационных Технологий И Защиты Информации");

            migrationBuilder.UpdateData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 3,
                column: "title",
                value: "Финансово-Экономическая Служба");

            migrationBuilder.UpdateData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 4,
                column: "title",
                value: "Отдел Медицинской Статистики И Мониторинга");

            migrationBuilder.UpdateData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 5,
                column: "title",
                value: "Отдел Информатизации Здравоохранения И Лекарственного Обеспечения");

            migrationBuilder.InsertData(
                table: "group",
                columns: new[] { "id_group", "title" },
                values: new object[,]
                {
                    { 6, "Директор" },
                    { 7, "Отдел Материально Технического Снабжения" },
                    { 8, "Отдел Реализации Национальных Проектов" },
                    { 9, "Отдел Консультирования И Аналитики В Сфере Закупок" },
                    { 10, "Региональный Ситуационный Центр По Вопросам Здравоохранения" },
                    { 11, "Дистанционный Консультативный Центр Лучевой Диагностики" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 11);

            migrationBuilder.UpdateData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 1,
                column: "title",
                value: "IT-Отдел");

            migrationBuilder.UpdateData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 3,
                column: "title",
                value: "Отдел закупок");

            migrationBuilder.UpdateData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 4,
                column: "title",
                value: "Служба безопасности");

            migrationBuilder.UpdateData(
                table: "group",
                keyColumn: "id_group",
                keyValue: 5,
                column: "title",
                value: "Юрист");
        }
    }
}
