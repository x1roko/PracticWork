using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TaskManagerMiac.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "group",
                columns: table => new
                {
                    id_group = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group", x => x.id_group);
                });

            migrationBuilder.CreateTable(
                name: "priority",
                columns: table => new
                {
                    id_priority = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    weight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priority", x => x.id_priority);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id_role = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id_role);
                });

            migrationBuilder.CreateTable(
                name: "task_state",
                columns: table => new
                {
                    id_task_state = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_state", x => x.id_task_state);
                });

            migrationBuilder.CreateTable(
                name: "task_type",
                columns: table => new
                {
                    id_task_type = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_type", x => x.id_task_type);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    snils = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    id_group = table.Column<int>(type: "integer", nullable: false),
                    firstname = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    surname = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    patronymic = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    id_role = table.Column<int>(type: "integer", nullable: false),
                    registration_date = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_active = table.Column<bool>(type: "bool", nullable: false, defaultValueSql: "true"),
                    last_enter_date = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id_user);
                    table.ForeignKey(
                        name: "fk_user_group",
                        column: x => x.id_group,
                        principalTable: "group",
                        principalColumn: "id_group");
                    table.ForeignKey(
                        name: "fk_user_role1",
                        column: x => x.id_role,
                        principalTable: "role",
                        principalColumn: "id_role");
                });

            migrationBuilder.CreateTable(
                name: "task_type_path",
                columns: table => new
                {
                    id_task_type = table.Column<int>(type: "integer", nullable: false),
                    id_group = table.Column<int>(type: "integer", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_type_path", x => new { x.id_task_type, x.id_group });
                    table.ForeignKey(
                        name: "fk_task_type_has_group_group1",
                        column: x => x.id_group,
                        principalTable: "group",
                        principalColumn: "id_group");
                    table.ForeignKey(
                        name: "fk_task_type_has_group_task_type1",
                        column: x => x.id_task_type,
                        principalTable: "task_type",
                        principalColumn: "id_task_type");
                });

            migrationBuilder.CreateTable(
                name: "task_body",
                columns: table => new
                {
                    id_task = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    id_user_creator = table.Column<int>(type: "integer", nullable: false),
                    id_priority = table.Column<int>(type: "integer", nullable: false),
                    id_task_type = table.Column<int>(type: "integer", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deadline_date = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_body", x => x.id_task);
                    table.ForeignKey(
                        name: "fk_task_body_task_type1",
                        column: x => x.id_task_type,
                        principalTable: "task_type",
                        principalColumn: "id_task_type");
                    table.ForeignKey(
                        name: "fk_task_priority1",
                        column: x => x.id_priority,
                        principalTable: "priority",
                        principalColumn: "id_priority");
                    table.ForeignKey(
                        name: "fk_task_user1",
                        column: x => x.id_user_creator,
                        principalTable: "user",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "document",
                columns: table => new
                {
                    id_document = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(105)", maxLength: 105, nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    extension = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    task_body_id_task = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document", x => x.id_document);
                    table.ForeignKey(
                        name: "fk_document_task_body1",
                        column: x => x.task_body_id_task,
                        principalTable: "task_body",
                        principalColumn: "id_task");
                });

            migrationBuilder.CreateTable(
                name: "task_body_path",
                columns: table => new
                {
                    task_body_id_task = table.Column<int>(type: "integer", nullable: false),
                    group_id_group = table.Column<int>(type: "integer", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_body_path", x => new { x.task_body_id_task, x.group_id_group });
                    table.ForeignKey(
                        name: "fk_task_body_has_group_group1",
                        column: x => x.group_id_group,
                        principalTable: "group",
                        principalColumn: "id_group");
                    table.ForeignKey(
                        name: "fk_task_body_has_group_task_body1",
                        column: x => x.task_body_id_task,
                        principalTable: "task_body",
                        principalColumn: "id_task");
                });

            migrationBuilder.CreateTable(
                name: "task_step",
                columns: table => new
                {
                    id_task_step = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false, defaultValueSql: "' '"),
                    id_task_state = table.Column<int>(type: "integer", nullable: false),
                    id_task = table.Column<int>(type: "integer", nullable: false),
                    group_id_performer = table.Column<int>(type: "integer", nullable: false),
                    deadline_date = table.Column<DateTime>(type: "timestamp", nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    change_state_date = table.Column<DateTime>(type: "timestamp", nullable: true),
                    performer_date = table.Column<DateTime>(type: "timestamp", nullable: true),
                    index = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_step", x => x.id_task_step);
                    table.ForeignKey(
                        name: "fk_task_step_group1",
                        column: x => x.group_id_performer,
                        principalTable: "group",
                        principalColumn: "id_group");
                    table.ForeignKey(
                        name: "fk_task_step_task1",
                        column: x => x.id_task,
                        principalTable: "task_body",
                        principalColumn: "id_task");
                    table.ForeignKey(
                        name: "fk_task_task_state1",
                        column: x => x.id_task_state,
                        principalTable: "task_state",
                        principalColumn: "id_task_state");
                });

            migrationBuilder.CreateTable(
                name: "user_has_task_step",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    id_task_step = table.Column<int>(type: "integer", nullable: false),
                    is_responsible = table.Column<bool>(type: "bool", nullable: false, defaultValueSql: "false")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_has_task_step", x => new { x.id_user, x.id_task_step });
                    table.ForeignKey(
                        name: "fk_user_has_task_step_task_step1",
                        column: x => x.id_task_step,
                        principalTable: "task_step",
                        principalColumn: "id_task_step");
                    table.ForeignKey(
                        name: "fk_user_has_task_step_user1",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateIndex(
                name: "fk_document_task_body1_idx",
                table: "document",
                column: "task_body_id_task");

            migrationBuilder.CreateIndex(
                name: "title_UNIQUE",
                table: "group",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "title_UNIQUE1",
                table: "priority",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "weight_UNIQUE",
                table: "priority",
                column: "weight",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "title_UNIQUE2",
                table: "role",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_task_body_task_type1_idx",
                table: "task_body",
                column: "id_task_type");

            migrationBuilder.CreateIndex(
                name: "fk_task_priority1_idx",
                table: "task_body",
                column: "id_priority");

            migrationBuilder.CreateIndex(
                name: "fk_task_user1_idx",
                table: "task_body",
                column: "id_user_creator");

            migrationBuilder.CreateIndex(
                name: "fk_task_body_has_group_group1_idx",
                table: "task_body_path",
                column: "group_id_group");

            migrationBuilder.CreateIndex(
                name: "fk_task_body_has_group_task_body1_idx",
                table: "task_body_path",
                column: "task_body_id_task");

            migrationBuilder.CreateIndex(
                name: "title_UNIQUE3",
                table: "task_state",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_task_step_group1_idx",
                table: "task_step",
                column: "group_id_performer");

            migrationBuilder.CreateIndex(
                name: "fk_task_step_task1_idx",
                table: "task_step",
                column: "id_task");

            migrationBuilder.CreateIndex(
                name: "fk_task_task_state1_idx",
                table: "task_step",
                column: "id_task_state");

            migrationBuilder.CreateIndex(
                name: "title_UNIQUE4",
                table: "task_type",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_task_type_has_group_group1_idx",
                table: "task_type_path",
                column: "id_group");

            migrationBuilder.CreateIndex(
                name: "fk_task_type_has_group_task_type1_idx",
                table: "task_type_path",
                column: "id_task_type");

            migrationBuilder.CreateIndex(
                name: "fk_user_group_idx",
                table: "user",
                column: "id_group");

            migrationBuilder.CreateIndex(
                name: "fk_user_role1_idx",
                table: "user",
                column: "id_role");

            migrationBuilder.CreateIndex(
                name: "snils_UNIQUE",
                table: "user",
                column: "snils",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_user_has_task_step_task_step1_idx",
                table: "user_has_task_step",
                column: "id_task_step");

            migrationBuilder.CreateIndex(
                name: "fk_user_has_task_step_user1_idx",
                table: "user_has_task_step",
                column: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "document");

            migrationBuilder.DropTable(
                name: "task_body_path");

            migrationBuilder.DropTable(
                name: "task_type_path");

            migrationBuilder.DropTable(
                name: "user_has_task_step");

            migrationBuilder.DropTable(
                name: "task_step");

            migrationBuilder.DropTable(
                name: "task_body");

            migrationBuilder.DropTable(
                name: "task_state");

            migrationBuilder.DropTable(
                name: "task_type");

            migrationBuilder.DropTable(
                name: "priority");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "group");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}
