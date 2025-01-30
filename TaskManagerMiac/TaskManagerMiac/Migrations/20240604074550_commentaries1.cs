using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class commentaries1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "task_body_comment",
                columns: table => new
                {
                    id_task_body_comment = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false),
                    id_task = table.Column<int>(type: "integer", nullable: false),
                    id_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_body_comment", x => x.id_task_body_comment);
                    table.ForeignKey(
                        name: "fk_task_has_commentaries",
                        column: x => x.id_task,
                        principalTable: "task_body",
                        principalColumn: "id_task");
                    table.ForeignKey(
                        name: "fk_user_has_commentaries",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateIndex(
                name: "IX_task_body_comment_id_task",
                table: "task_body_comment",
                column: "id_task");

            migrationBuilder.CreateIndex(
                name: "IX_task_body_comment_id_user",
                table: "task_body_comment",
                column: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "task_body_comment");
        }
    }
}
