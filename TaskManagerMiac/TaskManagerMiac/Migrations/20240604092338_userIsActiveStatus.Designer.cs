﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TaskManagerMiac.Data;

#nullable disable

namespace TaskManagerMiac.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240604092338_userIsActiveStatus")]
    partial class userIsActiveStatus
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TaskManagerMiac.Models.Document", b =>
                {
                    b.Property<int>("IdDocument")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_document");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdDocument"));

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasColumnName("created_on")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Extension")
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("extension");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<int>("TaskBodyIdTask")
                        .HasColumnType("integer")
                        .HasColumnName("task_body_id_task");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(105)
                        .HasColumnType("character varying(105)")
                        .HasColumnName("title");

                    b.HasKey("IdDocument");

                    b.HasIndex(new[] { "TaskBodyIdTask" }, "fk_document_task_body1_idx");

                    b.ToTable("document", (string)null);
                });

            modelBuilder.Entity("TaskManagerMiac.Models.FinancialTaskDocument", b =>
                {
                    b.Property<int>("IdDocument")
                        .HasColumnType("integer")
                        .HasColumnName("id_document");

                    b.Property<string>("Amount")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("amount");

                    b.Property<string>("DeliverDate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("deliver_date");

                    b.Property<string>("DeliverPlace")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("deliver_place");

                    b.Property<string>("FinanceSource")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("finance_source");

                    b.Property<string>("Guarantee")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("guarantee");

                    b.Property<string>("KBK")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("kbk");

                    b.Property<string>("KVR")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("kvr");

                    b.Property<string>("Law")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("law");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("notes");

                    b.Property<string>("OKPD")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("okpd");

                    b.Property<string>("Price")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("price");

                    b.Property<string>("Product")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("product");

                    b.HasKey("IdDocument");

                    b.ToTable("financial_task_document", (string)null);
                });

            modelBuilder.Entity("TaskManagerMiac.Models.Group", b =>
                {
                    b.Property<int>("IdGroup")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_group");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdGroup"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("title");

                    b.HasKey("IdGroup");

                    b.HasIndex(new[] { "Title" }, "title_UNIQUE")
                        .IsUnique();

                    b.ToTable("group", (string)null);

                    b.HasData(
                        new
                        {
                            IdGroup = 1,
                            Title = "IT-Отдел"
                        },
                        new
                        {
                            IdGroup = 2,
                            Title = "Не определён"
                        },
                        new
                        {
                            IdGroup = 3,
                            Title = "Отдел закупок"
                        },
                        new
                        {
                            IdGroup = 4,
                            Title = "Служба безопасности"
                        },
                        new
                        {
                            IdGroup = 5,
                            Title = "Юрист"
                        });
                });

            modelBuilder.Entity("TaskManagerMiac.Models.Priority", b =>
                {
                    b.Property<int>("IdPriority")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_priority");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdPriority"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("title");

                    b.Property<int>("Weight")
                        .HasColumnType("integer")
                        .HasColumnName("weight");

                    b.HasKey("IdPriority");

                    b.HasIndex(new[] { "Title" }, "title_UNIQUE")
                        .IsUnique()
                        .HasDatabaseName("title_UNIQUE1");

                    b.HasIndex(new[] { "Weight" }, "weight_UNIQUE")
                        .IsUnique();

                    b.ToTable("priority", (string)null);

                    b.HasData(
                        new
                        {
                            IdPriority = 1,
                            Title = "Низкий",
                            Weight = 0
                        },
                        new
                        {
                            IdPriority = 2,
                            Title = "Средний",
                            Weight = 50
                        },
                        new
                        {
                            IdPriority = 3,
                            Title = "Высокий",
                            Weight = 100
                        },
                        new
                        {
                            IdPriority = 4,
                            Title = "Максимальный",
                            Weight = 1000
                        });
                });

            modelBuilder.Entity("TaskManagerMiac.Models.Role", b =>
                {
                    b.Property<int>("IdRole")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_role");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdRole"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("title");

                    b.HasKey("IdRole");

                    b.HasIndex(new[] { "Title" }, "title_UNIQUE")
                        .IsUnique()
                        .HasDatabaseName("title_UNIQUE2");

                    b.ToTable("role", (string)null);

                    b.HasData(
                        new
                        {
                            IdRole = 1,
                            Title = "root"
                        },
                        new
                        {
                            IdRole = 2,
                            Title = "admin"
                        },
                        new
                        {
                            IdRole = 3,
                            Title = "group_admin"
                        },
                        new
                        {
                            IdRole = 4,
                            Title = "default"
                        });
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskBody", b =>
                {
                    b.Property<int>("IdTask")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_task");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdTask"));

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasColumnName("creation_date")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime?>("DeadlineDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("deadline_date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("description");

                    b.Property<int>("IdPriority")
                        .HasColumnType("integer")
                        .HasColumnName("id_priority");

                    b.Property<int>("IdTaskType")
                        .HasColumnType("integer")
                        .HasColumnName("id_task_type");

                    b.Property<int>("IdUserCreator")
                        .HasColumnType("integer")
                        .HasColumnName("id_user_creator");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("title");

                    b.HasKey("IdTask");

                    b.HasIndex(new[] { "IdTaskType" }, "fk_task_body_task_type1_idx");

                    b.HasIndex(new[] { "IdPriority" }, "fk_task_priority1_idx");

                    b.HasIndex(new[] { "IdUserCreator" }, "fk_task_user1_idx");

                    b.ToTable("task_body", (string)null);
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskBodyComment", b =>
                {
                    b.Property<int>("IdComment")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_task_body_comment");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdComment"));

                    b.Property<int>("IdTask")
                        .HasColumnType("integer")
                        .HasColumnName("id_task");

                    b.Property<int>("IdUser")
                        .HasColumnType("integer")
                        .HasColumnName("id_user");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("text");

                    b.HasKey("IdComment");

                    b.HasIndex("IdTask");

                    b.HasIndex("IdUser");

                    b.ToTable("task_body_comment", (string)null);
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskBodyPath", b =>
                {
                    b.Property<int>("TaskBodyIdTask")
                        .HasColumnType("integer")
                        .HasColumnName("task_body_id_task");

                    b.Property<int>("GroupIdGroup")
                        .HasColumnType("integer")
                        .HasColumnName("group_id_group");

                    b.Property<int>("Index")
                        .HasColumnType("integer")
                        .HasColumnName("index");

                    b.HasKey("TaskBodyIdTask", "GroupIdGroup");

                    b.HasIndex(new[] { "GroupIdGroup" }, "fk_task_body_has_group_group1_idx");

                    b.HasIndex(new[] { "TaskBodyIdTask" }, "fk_task_body_has_group_task_body1_idx");

                    b.ToTable("task_body_path", (string)null);
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskState", b =>
                {
                    b.Property<int>("IdTaskState")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_task_state");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdTaskState"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("title");

                    b.HasKey("IdTaskState");

                    b.HasIndex(new[] { "Title" }, "title_UNIQUE")
                        .IsUnique()
                        .HasDatabaseName("title_UNIQUE3");

                    b.ToTable("task_state", (string)null);

                    b.HasData(
                        new
                        {
                            IdTaskState = 1,
                            Title = "Создана"
                        },
                        new
                        {
                            IdTaskState = 2,
                            Title = "В работе"
                        },
                        new
                        {
                            IdTaskState = 3,
                            Title = "Одобрена"
                        },
                        new
                        {
                            IdTaskState = 4,
                            Title = "Отклонена"
                        },
                        new
                        {
                            IdTaskState = 5,
                            Title = "Отменена пользователем"
                        });
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskStep", b =>
                {
                    b.Property<int>("IdTaskStep")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_task_step");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdTaskStep"));

                    b.Property<DateTime?>("ChangeStateDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("change_state_date");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasColumnName("creation_date")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime?>("DeadlineDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("deadline_date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("description")
                        .HasDefaultValueSql("' '");

                    b.Property<int>("GroupIdPerformer")
                        .HasColumnType("integer")
                        .HasColumnName("group_id_performer");

                    b.Property<int>("IdTask")
                        .HasColumnType("integer")
                        .HasColumnName("id_task");

                    b.Property<int>("IdTaskState")
                        .HasColumnType("integer")
                        .HasColumnName("id_task_state");

                    b.Property<int>("Index")
                        .HasColumnType("integer")
                        .HasColumnName("index");

                    b.Property<DateTime?>("PerformerDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("performer_date");

                    b.HasKey("IdTaskStep");

                    b.HasIndex(new[] { "GroupIdPerformer" }, "fk_task_step_group1_idx");

                    b.HasIndex(new[] { "IdTask" }, "fk_task_step_task1_idx");

                    b.HasIndex(new[] { "IdTaskState" }, "fk_task_task_state1_idx");

                    b.ToTable("task_step", (string)null);
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskType", b =>
                {
                    b.Property<int>("IdTaskType")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_task_type");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdTaskType"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("title");

                    b.HasKey("IdTaskType");

                    b.HasIndex(new[] { "Title" }, "title_UNIQUE")
                        .IsUnique()
                        .HasDatabaseName("title_UNIQUE4");

                    b.ToTable("task_type", (string)null);
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskTypePath", b =>
                {
                    b.Property<int>("IdTaskType")
                        .HasColumnType("integer")
                        .HasColumnName("id_task_type");

                    b.Property<int>("IdGroup")
                        .HasColumnType("integer")
                        .HasColumnName("id_group");

                    b.Property<int>("Index")
                        .HasColumnType("integer")
                        .HasColumnName("index");

                    b.HasKey("IdTaskType", "IdGroup");

                    b.HasIndex(new[] { "IdGroup" }, "fk_task_type_has_group_group1_idx");

                    b.HasIndex(new[] { "IdTaskType" }, "fk_task_type_has_group_task_type1_idx");

                    b.ToTable("task_type_path", (string)null);
                });

            modelBuilder.Entity("TaskManagerMiac.Models.User", b =>
                {
                    b.Property<int>("IdUser")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_user");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdUser"));

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("firstname");

                    b.Property<int>("IdGroup")
                        .HasColumnType("integer")
                        .HasColumnName("id_group");

                    b.Property<int>("IdRole")
                        .HasColumnType("integer")
                        .HasColumnName("id_role");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasColumnName("is_active")
                        .HasDefaultValueSql("false");

                    b.Property<DateTime>("LastEnterDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasColumnName("last_enter_date")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Patronymic")
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("patronymic");

                    b.Property<DateTime>("RegistrationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasColumnName("registration_date")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Snils")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)")
                        .HasColumnName("snils");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)")
                        .HasColumnName("surname");

                    b.HasKey("IdUser");

                    b.HasIndex(new[] { "IdGroup" }, "fk_user_group_idx");

                    b.HasIndex(new[] { "IdRole" }, "fk_user_role1_idx");

                    b.HasIndex(new[] { "Snils" }, "snils_UNIQUE")
                        .IsUnique();

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("TaskManagerMiac.Models.UserHasTaskStep", b =>
                {
                    b.Property<int>("IdUser")
                        .HasColumnType("integer")
                        .HasColumnName("id_user");

                    b.Property<int>("IdTaskStep")
                        .HasColumnType("integer")
                        .HasColumnName("id_task_step");

                    b.Property<bool>("IsChecked")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasColumnName("is_checked")
                        .HasDefaultValueSql("false");

                    b.Property<bool>("IsResponsible")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bool")
                        .HasColumnName("is_responsible")
                        .HasDefaultValueSql("false");

                    b.HasKey("IdUser", "IdTaskStep");

                    b.HasIndex(new[] { "IdTaskStep" }, "fk_user_has_task_step_task_step1_idx");

                    b.HasIndex(new[] { "IdUser" }, "fk_user_has_task_step_user1_idx");

                    b.ToTable("user_has_task_step", (string)null);
                });

            modelBuilder.Entity("TaskManagerMiac.Models.Document", b =>
                {
                    b.HasOne("TaskManagerMiac.Models.TaskBody", "TaskBodyIdTaskNavigation")
                        .WithMany("Documents")
                        .HasForeignKey("TaskBodyIdTask")
                        .IsRequired()
                        .HasConstraintName("fk_document_task_body1");

                    b.Navigation("TaskBodyIdTaskNavigation");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.FinancialTaskDocument", b =>
                {
                    b.HasOne("TaskManagerMiac.Models.Document", "IdDocumentNavigation")
                        .WithOne("FinancialTaskDocumentNavigation")
                        .HasForeignKey("TaskManagerMiac.Models.FinancialTaskDocument", "IdDocument")
                        .IsRequired()
                        .HasConstraintName("fk_financial_task_document_has_document");

                    b.Navigation("IdDocumentNavigation");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskBody", b =>
                {
                    b.HasOne("TaskManagerMiac.Models.Priority", "IdPriorityNavigation")
                        .WithMany("TaskBodies")
                        .HasForeignKey("IdPriority")
                        .IsRequired()
                        .HasConstraintName("fk_task_priority1");

                    b.HasOne("TaskManagerMiac.Models.TaskType", "IdTaskTypeNavigation")
                        .WithMany("TaskBodies")
                        .HasForeignKey("IdTaskType")
                        .IsRequired()
                        .HasConstraintName("fk_task_body_task_type1");

                    b.HasOne("TaskManagerMiac.Models.User", "IdUserCreatorNavigation")
                        .WithMany("TaskBodies")
                        .HasForeignKey("IdUserCreator")
                        .IsRequired()
                        .HasConstraintName("fk_task_user1");

                    b.Navigation("IdPriorityNavigation");

                    b.Navigation("IdTaskTypeNavigation");

                    b.Navigation("IdUserCreatorNavigation");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskBodyComment", b =>
                {
                    b.HasOne("TaskManagerMiac.Models.TaskBody", "IdTaskNavigation")
                        .WithMany("Commentaries")
                        .HasForeignKey("IdTask")
                        .IsRequired()
                        .HasConstraintName("fk_task_has_commentaries");

                    b.HasOne("TaskManagerMiac.Models.User", "IdUserNavigation")
                        .WithMany("Commentaries")
                        .HasForeignKey("IdUser")
                        .IsRequired()
                        .HasConstraintName("fk_user_has_commentaries");

                    b.Navigation("IdTaskNavigation");

                    b.Navigation("IdUserNavigation");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskBodyPath", b =>
                {
                    b.HasOne("TaskManagerMiac.Models.Group", "GroupIdGroupNavigation")
                        .WithMany("TaskBodyPaths")
                        .HasForeignKey("GroupIdGroup")
                        .IsRequired()
                        .HasConstraintName("fk_task_body_has_group_group1");

                    b.HasOne("TaskManagerMiac.Models.TaskBody", "TaskBodyIdTaskNavigation")
                        .WithMany("TaskBodyPaths")
                        .HasForeignKey("TaskBodyIdTask")
                        .IsRequired()
                        .HasConstraintName("fk_task_body_has_group_task_body1");

                    b.Navigation("GroupIdGroupNavigation");

                    b.Navigation("TaskBodyIdTaskNavigation");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskStep", b =>
                {
                    b.HasOne("TaskManagerMiac.Models.Group", "GroupIdPerformerNavigation")
                        .WithMany("TaskSteps")
                        .HasForeignKey("GroupIdPerformer")
                        .IsRequired()
                        .HasConstraintName("fk_task_step_group1");

                    b.HasOne("TaskManagerMiac.Models.TaskBody", "IdTaskNavigation")
                        .WithMany("TaskSteps")
                        .HasForeignKey("IdTask")
                        .IsRequired()
                        .HasConstraintName("fk_task_step_task1");

                    b.HasOne("TaskManagerMiac.Models.TaskState", "IdTaskStateNavigation")
                        .WithMany("TaskSteps")
                        .HasForeignKey("IdTaskState")
                        .IsRequired()
                        .HasConstraintName("fk_task_task_state1");

                    b.Navigation("GroupIdPerformerNavigation");

                    b.Navigation("IdTaskNavigation");

                    b.Navigation("IdTaskStateNavigation");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskTypePath", b =>
                {
                    b.HasOne("TaskManagerMiac.Models.Group", "IdGroupNavigation")
                        .WithMany("TaskTypePaths")
                        .HasForeignKey("IdGroup")
                        .IsRequired()
                        .HasConstraintName("fk_task_type_has_group_group1");

                    b.HasOne("TaskManagerMiac.Models.TaskType", "IdTaskTypeNavigation")
                        .WithMany("TaskTypePaths")
                        .HasForeignKey("IdTaskType")
                        .IsRequired()
                        .HasConstraintName("fk_task_type_has_group_task_type1");

                    b.Navigation("IdGroupNavigation");

                    b.Navigation("IdTaskTypeNavigation");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.User", b =>
                {
                    b.HasOne("TaskManagerMiac.Models.Group", "IdGroupNavigation")
                        .WithMany("Users")
                        .HasForeignKey("IdGroup")
                        .IsRequired()
                        .HasConstraintName("fk_user_group");

                    b.HasOne("TaskManagerMiac.Models.Role", "IdRoleNavigation")
                        .WithMany("Users")
                        .HasForeignKey("IdRole")
                        .IsRequired()
                        .HasConstraintName("fk_user_role1");

                    b.Navigation("IdGroupNavigation");

                    b.Navigation("IdRoleNavigation");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.UserHasTaskStep", b =>
                {
                    b.HasOne("TaskManagerMiac.Models.TaskStep", "IdTaskStepNavigation")
                        .WithMany("UserHasTaskSteps")
                        .HasForeignKey("IdTaskStep")
                        .IsRequired()
                        .HasConstraintName("fk_user_has_task_step_task_step1");

                    b.HasOne("TaskManagerMiac.Models.User", "IdUserNavigation")
                        .WithMany("UserHasTaskSteps")
                        .HasForeignKey("IdUser")
                        .IsRequired()
                        .HasConstraintName("fk_user_has_task_step_user1");

                    b.Navigation("IdTaskStepNavigation");

                    b.Navigation("IdUserNavigation");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.Document", b =>
                {
                    b.Navigation("FinancialTaskDocumentNavigation")
                        .IsRequired();
                });

            modelBuilder.Entity("TaskManagerMiac.Models.Group", b =>
                {
                    b.Navigation("TaskBodyPaths");

                    b.Navigation("TaskSteps");

                    b.Navigation("TaskTypePaths");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.Priority", b =>
                {
                    b.Navigation("TaskBodies");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskBody", b =>
                {
                    b.Navigation("Commentaries");

                    b.Navigation("Documents");

                    b.Navigation("TaskBodyPaths");

                    b.Navigation("TaskSteps");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskState", b =>
                {
                    b.Navigation("TaskSteps");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskStep", b =>
                {
                    b.Navigation("UserHasTaskSteps");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.TaskType", b =>
                {
                    b.Navigation("TaskBodies");

                    b.Navigation("TaskTypePaths");
                });

            modelBuilder.Entity("TaskManagerMiac.Models.User", b =>
                {
                    b.Navigation("Commentaries");

                    b.Navigation("TaskBodies");

                    b.Navigation("UserHasTaskSteps");
                });
#pragma warning restore 612, 618
        }
    }
}
