using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TaskManagerMiac.Models;

namespace TaskManagerMiac.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TaskBody> TaskBodies { get; set; }

    public virtual DbSet<TaskBodyPath> TaskBodyPaths { get; set; }

    public virtual DbSet<TaskState> TaskStates { get; set; }

    public virtual DbSet<TaskStep> TaskSteps { get; set; }

    public virtual DbSet<TaskType> TaskTypes { get; set; }

    public virtual DbSet<TaskTypePath> TaskTypePaths { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserHasTaskStep> UserHasTaskSteps { get; set; }
    public virtual DbSet<TaskBodyComment> TaskBodyComments { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseMySQL("server=softsols.online;port=3306;user=root;password=example;database=task_manager_miac2");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.IdDocument);

            entity.ToTable("document");

            entity.HasIndex(e => e.TaskBodyIdTask, "fk_document_task_body1_idx");

            entity.Property(e => e.IdDocument).HasColumnName("id_document");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_on");
            entity.Property(e => e.Extension)
                .HasMaxLength(45)
                .HasColumnName("extension");
            entity.Property(e => e.TaskBodyIdTask).HasColumnName("task_body_id_task");
            entity.Property(e => e.Title)
                .HasMaxLength(105)
                .HasColumnName("title");

            entity.HasOne(d => d.TaskBodyIdTaskNavigation).WithMany(p => p.Documents)
                .HasForeignKey(d => d.TaskBodyIdTask)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_document_task_body1");
        });
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.IdGroup);

            entity.ToTable("group");

            entity.HasIndex(e => e.Title, "title_UNIQUE").IsUnique();

            entity.Property(e => e.IdGroup).HasColumnName("id_group");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.HasData(InitialDbData.Groups);
        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.IdPriority);

            entity.ToTable("priority");

            entity.HasIndex(e => e.Title, "title_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Weight, "weight_UNIQUE").IsUnique();

            entity.Property(e => e.IdPriority).HasColumnName("id_priority");
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .HasColumnName("title");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.HasData(InitialDbData.Priorities);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole);

            entity.ToTable("role");

            entity.HasIndex(e => e.Title, "title_UNIQUE").IsUnique();

            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .HasColumnName("title");
            entity.HasData(InitialDbData.Roles);
        });

        modelBuilder.Entity<TaskBody>(entity =>
        {
            entity.HasKey(e => e.IdTask);

            entity.ToTable("task_body");

            entity.HasIndex(e => e.IdTaskType, "fk_task_body_task_type1_idx");

            entity.HasIndex(e => e.IdPriority, "fk_task_priority1_idx");

            entity.HasIndex(e => e.IdUserCreator, "fk_task_user1_idx");

            entity.Property(e => e.IdTask).HasColumnName("id_task");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("creation_date");
            entity.Property(e => e.DeadlineDate)
                .HasColumnType("timestamp")
                .HasColumnName("deadline_date");
            entity.Property(e => e.Description)
                .HasMaxLength(2000)
                .HasColumnName("description");
            entity.Property(e => e.IdPriority).HasColumnName("id_priority");
            entity.Property(e => e.IdTaskType).HasColumnName("id_task_type");
            entity.Property(e => e.IdUserCreator).HasColumnName("id_user_creator");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasOne(d => d.IdPriorityNavigation).WithMany(p => p.TaskBodies)
                .HasForeignKey(d => d.IdPriority)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_priority1");

            entity.HasOne(d => d.IdTaskTypeNavigation).WithMany(p => p.TaskBodies)
                .HasForeignKey(d => d.IdTaskType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_body_task_type1");

            entity.HasOne(d => d.IdUserCreatorNavigation).WithMany(p => p.TaskBodies)
                .HasForeignKey(d => d.IdUserCreator)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_user1");
        });

        modelBuilder.Entity<TaskBodyPath>(entity =>
        {
            entity.HasKey(e => new { e.TaskBodyIdTask, e.GroupIdGroup });

            entity.ToTable("task_body_path");

            entity.HasIndex(e => e.GroupIdGroup, "fk_task_body_has_group_group1_idx");

            entity.HasIndex(e => e.TaskBodyIdTask, "fk_task_body_has_group_task_body1_idx");

            entity.Property(e => e.TaskBodyIdTask).HasColumnName("task_body_id_task");
            entity.Property(e => e.GroupIdGroup).HasColumnName("group_id_group");
            entity.Property(e => e.Index).HasColumnName("index");

            entity.HasOne(d => d.GroupIdGroupNavigation).WithMany(p => p.TaskBodyPaths)
                .HasForeignKey(d => d.GroupIdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_body_has_group_group1");

            entity.HasOne(d => d.TaskBodyIdTaskNavigation).WithMany(p => p.TaskBodyPaths)
                .HasForeignKey(d => d.TaskBodyIdTask)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_body_has_group_task_body1");
        });

        modelBuilder.Entity<TaskState>(entity =>
        {
            entity.HasKey(e => e.IdTaskState);

            entity.ToTable("task_state");

            entity.HasIndex(e => e.Title, "title_UNIQUE").IsUnique();

            entity.Property(e => e.IdTaskState).HasColumnName("id_task_state");
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .HasColumnName("title");
            entity.HasData(InitialDbData.TaskStates);
        }); 

        modelBuilder.Entity<TaskStep>(entity =>
        {
            entity.HasKey(e => e.IdTaskStep);

            entity.ToTable("task_step");

            entity.HasIndex(e => e.GroupIdPerformer, "fk_task_step_group1_idx");

            entity.HasIndex(e => e.IdTask, "fk_task_step_task1_idx");

            entity.HasIndex(e => e.IdTaskState, "fk_task_task_state1_idx");

            entity.Property(e => e.IdTaskStep).HasColumnName("id_task_step");
            entity.Property(e => e.ChangeStateDate)
                .HasColumnType("timestamp")
                .HasColumnName("change_state_date");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("creation_date");
            entity.Property(e => e.DeadlineDate)
                .HasColumnType("timestamp")
                .HasColumnName("deadline_date");
            entity.Property(e => e.Description)
                .HasMaxLength(45)
                .HasDefaultValueSql("' '")
                .HasColumnName("description");
            entity.Property(e => e.GroupIdPerformer).HasColumnName("group_id_performer");
            entity.Property(e => e.IdTask).HasColumnName("id_task");
            entity.Property(e => e.IdTaskState).HasColumnName("id_task_state");
            entity.Property(e => e.Index).HasColumnName("index");
            entity.Property(e => e.PerformerDate)
                .HasColumnType("timestamp")
                .HasColumnName("performer_date");

            entity.HasOne(d => d.GroupIdPerformerNavigation).WithMany(p => p.TaskSteps)
                .HasForeignKey(d => d.GroupIdPerformer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_step_group1");

            entity.HasOne(d => d.IdTaskNavigation).WithMany(p => p.TaskSteps)
                .HasForeignKey(d => d.IdTask)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_step_task1");

            entity.HasOne(d => d.IdTaskStateNavigation).WithMany(p => p.TaskSteps)
                .HasForeignKey(d => d.IdTaskState)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_task_state1");
        });

        modelBuilder.Entity<TaskType>(entity =>
        {
            entity.HasKey(e => e.IdTaskType);

            entity.ToTable("task_type");

            entity.HasIndex(e => e.Title, "title_UNIQUE").IsUnique();

            entity.Property(e => e.IdTaskType).HasColumnName("id_task_type");
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .HasColumnName("title");
            entity.HasData(InitialDbData.TaskTypes);
        });

        modelBuilder.Entity<TaskTypePath>(entity =>
        {
            entity.HasKey(e => new { e.IdTaskType, e.IdGroup });

            entity.ToTable("task_type_path");

            entity.HasIndex(e => e.IdGroup, "fk_task_type_has_group_group1_idx");

            entity.HasIndex(e => e.IdTaskType, "fk_task_type_has_group_task_type1_idx");

            entity.Property(e => e.IdTaskType).HasColumnName("id_task_type");
            entity.Property(e => e.IdGroup).HasColumnName("id_group");
            entity.Property(e => e.Index).HasColumnName("index");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.TaskTypePaths)
                .HasForeignKey(d => d.IdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_type_has_group_group1");

            entity.HasOne(d => d.IdTaskTypeNavigation).WithMany(p => p.TaskTypePaths)
                .HasForeignKey(d => d.IdTaskType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_type_has_group_task_type1");
            entity.HasData(InitialDbData.TaskTypePaths);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("user");

            entity.HasIndex(e => e.IdGroup, "fk_user_group_idx");

            entity.HasIndex(e => e.IdRole, "fk_user_role1_idx");

            entity.HasIndex(e => e.Snils, "snils_UNIQUE").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Firstname)
                .HasMaxLength(45)
                .HasColumnName("firstname");
            entity.Property(e => e.IdGroup).HasColumnName("id_group");
            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("false")
                .HasColumnType("bool")
                .HasColumnName("is_active");
            entity.Property(e => e.LastEnterDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("last_enter_date");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(45)
                .HasColumnName("patronymic");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("registration_date");
            entity.Property(e => e.Snils)
                .HasColumnName("snils");
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .HasColumnName("surname");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_group");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_role1");
            entity.HasData(InitialDbData.Users);
        });

        modelBuilder.Entity<UserHasTaskStep>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdTaskStep });

            entity.ToTable("user_has_task_step");

            entity.HasIndex(e => e.IdTaskStep, "fk_user_has_task_step_task_step1_idx");

            entity.HasIndex(e => e.IdUser, "fk_user_has_task_step_user1_idx");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IdTaskStep).HasColumnName("id_task_step");
            entity.Property(e => e.IsResponsible)
                .HasDefaultValueSql("false")
                .HasColumnType("bool")
                .HasColumnName("is_responsible");

            entity.Property(e => e.IsChecked)
                .HasDefaultValueSql("false")
                .HasColumnType("bool")
                .HasColumnName("is_checked");

            entity.HasOne(d => d.IdTaskStepNavigation).WithMany(p => p.UserHasTaskSteps)
                .HasForeignKey(d => d.IdTaskStep)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_has_task_step_task_step1");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserHasTaskSteps)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_has_task_step_user1");
        });
        modelBuilder.Entity<FinancialTaskDocument>(entity =>
        {
            entity.ToTable("financial_task_document");
            entity.HasKey(e => e.IdDocument);

            entity.Property(e => e.IdDocument).HasColumnName("id_document");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OKPD).HasColumnName("okpd");
            entity.Property(e => e.Product).HasColumnName("product");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.DeliverDate).HasColumnName("deliver_date");
            entity.Property(e => e.Guarantee).HasColumnName("guarantee");
            entity.Property(e => e.DeliverPlace).HasColumnName("deliver_place");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.KVR).HasColumnName("kvr");
            entity.Property(e => e.KBK).HasColumnName("kbk");
            entity.Property(e => e.Law).HasColumnName("law");
            entity.Property(e => e.FinanceSource).HasColumnName("finance_source");

            entity.HasOne(d => d.IdDocumentNavigation).WithOne(p => p.FinancialTaskDocumentNavigation)
                .HasForeignKey<FinancialTaskDocument>(f => f.IdDocument)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_financial_task_document_has_document")
                .IsRequired();
        });

        modelBuilder.Entity<TaskBodyComment>(entity =>
        {
            entity.ToTable("task_body_comment");
            entity.HasKey(e => e.IdComment);

            entity.Property(e => e.IdComment).HasColumnName("id_task_body_comment");
            entity.Property(e => e.IdTask).HasColumnName("id_task");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.PostDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("post_date");

            entity.HasOne(c => c.IdUserNavigation).WithMany(u => u.Commentaries)
                .HasForeignKey(t => t.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_has_commentaries")
                .IsRequired();

            entity.HasOne(c => c.IdTaskNavigation).WithMany(u => u.Commentaries)
                .HasForeignKey(t => t.IdTask)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_task_has_commentaries")
                .IsRequired();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
