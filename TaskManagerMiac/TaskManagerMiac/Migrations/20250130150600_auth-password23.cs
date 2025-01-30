using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class authpassword23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id_user", "firstname", "id_group", "id_role", "is_active", "Password", "patronymic", "snils", "surname" },
                values: new object[] { 1, "Администратор", 1, 1, true, "$2a$11$cT8irG7/Ag.RKhxa48PWq.QVQR6jQHVefczIHo.lPs.ryau8OQVfW", null, "admin@a.ru", " " });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user",
                keyColumn: "id_user",
                keyValue: 1);
        }
    }
}
