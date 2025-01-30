using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class Financial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "financial_task_document",
                columns: table => new
                {
                    id_document = table.Column<int>(type: "integer", nullable: false),
                    product = table.Column<string>(type: "text", nullable: false),
                    okpd = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<string>(type: "text", nullable: false),
                    deliver_place = table.Column<string>(type: "text", nullable: false),
                    deliver_date = table.Column<string>(type: "text", nullable: false),
                    Guarantee = table.Column<string>(type: "text", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_task_document", x => x.id_document);
                    table.ForeignKey(
                        name: "fk_financial_task_document_has_document",
                        column: x => x.id_document,
                        principalTable: "document",
                        principalColumn: "id_document");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "financial_task_document");
        }
    }
}
