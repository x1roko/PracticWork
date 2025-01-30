using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class userIsActiveStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "user",
                type: "bool",
                nullable: false,
                defaultValueSql: "false",
                oldClrType: typeof(bool),
                oldType: "bool",
                oldDefaultValueSql: "true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "user",
                type: "bool",
                nullable: false,
                defaultValueSql: "true",
                oldClrType: typeof(bool),
                oldType: "bool",
                oldDefaultValueSql: "false");
        }
    }
}
