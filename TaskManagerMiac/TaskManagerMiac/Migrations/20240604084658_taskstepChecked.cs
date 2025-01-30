using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class taskstepChecked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_checked",
                table: "user_has_task_step",
                type: "bool",
                nullable: false,
                defaultValueSql: "false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_checked",
                table: "user_has_task_step");
        }
    }
}
