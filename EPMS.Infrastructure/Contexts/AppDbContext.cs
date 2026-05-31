using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EPMS.Infrastructure.Contexts;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationForm> ApplicationForms { get; set; }

    public virtual DbSet<AppraisalCycle> AppraisalCycles { get; set; }

    public virtual DbSet<AppraisalQuestion> AppraisalQuestions { get; set; }

    public virtual DbSet<AppraisalResponse> AppraisalResponses { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeInfo> EmployeeInfos { get; set; }

    public virtual DbSet<FormQuestion> FormQuestions { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<MeetingNote> MeetingNotes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<OneOnOneMeeting> OneOnOneMeetings { get; set; }

    public virtual DbSet<PerformanceEvaluation> PerformanceEvaluations { get; set; }

    public virtual DbSet<PerformanceOutcome> PerformanceOutcomes { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PipMeeting> PipMeetings { get; set; }

    public virtual DbSet<PipObjective> PipObjectives { get; set; }

    public virtual DbSet<PipPlan> PipPlans { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<PositionPermission> PositionPermissions { get; set; }

    public virtual DbSet<RatingScale> RatingScales { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<PositionKpi> PositionKpis { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<SystemSetting> SystemSettings { get; set; }
    public virtual DbSet<Kpi> Kpis { get; set; }
    public virtual DbSet<DepartmentKpi> DepartmentKpis { get; set; }
    public virtual DbSet<TeamKpi> TeamKpis { get; set; }
    public virtual DbSet<EmployeeKpi> EmployeeKpis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // 3. DepartmentKpi -> Kpi (Master)
        modelBuilder.Entity<Kpi>(entity =>
        {
            entity.ToTable("KPIs");
            entity.HasKey(k => k.KpiId);
            entity.Property(k => k.KpiId).HasColumnName("KpiID");
            entity.Property(k => k.KpiName).HasColumnName("KPIName").IsRequired(false).HasMaxLength(255);
            entity.Property(k => k.Direction).HasConversion<int>().IsRequired(false);
            
            // Ignore properties not in database
            entity.Ignore(k => k.CreatedAt);
            entity.Ignore(k => k.CreatedByEmployeeId);
            entity.Property(k => k.IsActive).IsRequired(false);
        });
                
        modelBuilder.Entity<DepartmentKpi>(entity =>
        {
            entity.ToTable("DepartmentKpis");
            entity.HasKey(d => d.DeptKpiId);
            entity.Property(d => d.DeptKpiId).HasColumnName("DeptKpiID");
            entity.Property(d => d.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(d => d.CycleId).HasColumnName("CycleID");
            
            entity.Property(d => d.DepartmentTarget)
                .HasColumnType("decimal(18, 4)")
                .IsRequired(false);
            
            entity.Property(d => d.ActualValue)
                .HasColumnType("decimal(18, 4)")
                .IsRequired(false);
            
            entity.Property(d => d.Weight)
                .HasColumnType("decimal(5, 2)")
                .IsRequired(false);
            
            // Ignore properties not in database
            entity.Ignore(d => d.IsActive);
            entity.Ignore(d => d.CreatedAt);
            
            entity.HasOne(d => d.Kpi)
                  .WithMany(p => p.DepartmentKpis)
                  .HasForeignKey(d => d.KpiId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Department)
                  .WithMany(p => p.DepartmentKpis)
                  .HasForeignKey(d => d.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Cycle)
                  .WithMany(p => p.DepartmentKpis)
                  .HasForeignKey(d => d.CycleId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.Property(d => d.KpiId).HasColumnName("KpiID");
        });

        // 4. TeamKpi
        modelBuilder.Entity<TeamKpi>(entity =>
        {
            entity.ToTable("TeamKPIs");
            entity.HasKey(t => t.TeamKpiId);
            entity.Property(t => t.TeamKpiId).HasColumnName("TeamKPIID");
            entity.Property(t => t.TeamId).HasColumnName("TeamID");
            entity.Property(t => t.DeptKpiId).HasColumnName("DeptKPIID");
            
            entity.Property(t => t.TeamTarget)
                .HasColumnType("decimal(18, 4)")
                .IsRequired(false);
            
            entity.Property(t => t.Weight)
                .HasColumnType("decimal(5, 2)")
                .IsRequired(false);

            entity.Property(t => t.ActualValue)
                .HasColumnType("decimal(18, 4)")
                .IsRequired(false);

            entity.Property(t => t.VersionNo).HasColumnName("VersionNo");
            entity.Property(t => t.SnapshotTarget).HasColumnType("decimal(18, 4)");
            entity.Property(t => t.SnapshotWeight).HasColumnType("decimal(5, 2)");
            entity.Property(t => t.ReasonForRevision).HasMaxLength(500);
            entity.Property(t => t.Status).HasMaxLength(20);

            entity.Property(t => t.ScorePercent)
                .HasColumnName("ScorePercent")
                .HasColumnType("decimal(18, 2)")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(t => t.WeightedScore)
                .HasColumnName("WeightedScore")
                .HasColumnType("decimal(18, 2)")
                .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(t => t.Team)
                  .WithMany(p => p.TeamKpis)
                  .HasForeignKey(t => t.TeamId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.DepartmentKpi)
                  .WithMany(p => p.TeamKpis)
                  .HasForeignKey(t => t.DeptKpiId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployeeKpi>(entity =>
        {
            entity.ToTable("EmployeeKpi");
            entity.HasKey(e => e.EmployeeKpiId);

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeId").IsRequired();
            entity.Property(e => e.CycleId).HasColumnName("CycleId").IsRequired();
            entity.Property(e => e.KpiId).HasColumnName("KpiId").IsRequired();
            entity.Property(e => e.PositionKpiId).HasColumnName("PositionKpiId").IsRequired(false);
            entity.Property(e => e.TeamKpiId).HasColumnName("TeamKpiId").IsRequired(false);

            entity.Property(e => e.Weight)
                .HasColumnType("decimal(5, 2)")
                .IsRequired(false);

            entity.Property(e => e.TargetValue)
                .HasColumnType("decimal(18, 2)")
                .IsRequired(false);

            entity.Property(e => e.ActualValue)
                .HasColumnType("decimal(18, 2)")
                .HasDefaultValue(0.00m)
                .IsRequired(false);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            entity.Property(e => e.Score)
                .HasColumnName("Score")
                .HasColumnType("decimal(18, 2)")
                .ValueGeneratedOnAddOrUpdate();

            entity.Property(e => e.WeightedScore)
                .HasColumnName("WeightedScore")
                .HasColumnType("decimal(18, 2)")
                .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(e => e.Employee)
                  .WithMany(p => p.EmployeeKpis)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AppraisalCycle)
                  .WithMany(p => p.EmployeeKpis)
                  .HasForeignKey(e => e.CycleId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Kpi)
                  .WithMany(p => p.EmployeeKpis)
                  .HasForeignKey(e => e.KpiId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PositionKpi)
                  .WithMany(p => p.EmployeeKpis)
                  .HasForeignKey(e => e.PositionKpiId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TeamKpi)
                  .WithMany(p => p.EmployeeKpis)
                  .HasForeignKey(e => e.TeamKpiId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApplicationForm>(entity =>
        {
            entity.HasKey(e => e.FormId).HasName("PK__Applicat__FB05B7BD0CD83A1E");

            entity.Property(e => e.FormId).HasColumnName("FormID");
            entity.Property(e => e.FormName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<AppraisalCycle>(entity =>
        {
            entity.HasKey(e => e.CycleId).HasName("PK__Appraisa__077B24D99AD1D91E");

            entity.Property(e => e.CycleId).HasColumnName("CycleID");
            entity.Property(e => e.CycleName).HasMaxLength(100);
            entity.Property(e => e.CycleStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.EvaluationPeriod).HasMaxLength(50);
        });

        modelBuilder.Entity<AppraisalQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Appraisa__0DC06F8C1D02B801");

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.IsRequired).HasDefaultValue(true);
        });

        modelBuilder.Entity<AppraisalResponse>(entity =>
        {
            entity.HasKey(e => e.ResponseId).HasName("PK__Appraisa__1AAA640CD4C09945");

            entity.Property(e => e.ResponseId).HasColumnName("ResponseID");
            entity.Property(e => e.RespondentRole).HasMaxLength(50).IsRequired();
            entity.Property(e => e.EvalId).HasColumnName("EvalID");
            entity.Property(e => e.IsAnonymous).HasDefaultValue(false);
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.RespondentId).HasColumnName("RespondentID");
            entity.Property(e => e.RespondentEmployeeId).HasColumnName("RespondentEmployeeID");

            entity.HasOne(d => d.Respondent) 
                .WithMany() 
          .     HasForeignKey(d => d.RespondentEmployeeId)
                .HasConstraintName("FK_AppraisalResponses_RespondentEmployee");

            entity.HasOne(d => d.Eval).WithMany(p => p.AppraisalResponses)
                .HasForeignKey(d => d.EvalId)
                .HasConstraintName("FK__Appraisal__EvalI__2645B050");

            entity.HasOne(d => d.Question).WithMany(p => p.AppraisalResponses)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__Appraisal__Quest__2739D489");

            entity.HasOne(d => d.Respondent).WithMany(p => p.AppraisalResponses)
                .HasForeignKey(d => d.RespondentId)
                .HasConstraintName("FK__Appraisal__Respo__282DF8C2");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F23B8D2EF09F9");

            entity.Property(e => e.AuditId).HasColumnName("AuditID");
            entity.Property(e => e.ActionType).HasMaxLength(10);
            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.ChangedByEmployeeId).HasColumnName("ChangedByEmployeeID");
            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.TableName).HasMaxLength(100);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCD16819445");

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentIdName).HasMaxLength(50);
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ParentDepartmentId).HasColumnName("ParentDepartmentID");

            entity.HasOne(d => d.ParentDepartment).WithMany(p => p.InverseParentDepartment)
                .HasForeignKey(d => d.ParentDepartmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Departments_Departments_Parent");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF19EDAB7BE");

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasIndex(e => e.EmployeeCode, "UQ_EmployeeCode").IsUnique();

            entity.HasIndex(e => e.EmployeeCode, "UQ__Employee__1F64254896E975DC").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.BankAccountNumber).HasMaxLength(50);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.CurrentSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmergencyContact).HasMaxLength(200);
            entity.Property(e => e.EmergencyPhone).HasMaxLength(20);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.EmploymentStatus).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.NrcNumber)
                .HasMaxLength(50)
                .HasColumnName("NRC_Number");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.PromotionEligibility).HasDefaultValue(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Employees__Depar__403A8C7D");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__Employees__Posit__412EB0B6");

            entity.HasOne(d => d.ReportsToNavigation).WithMany(p => p.InverseReportsToNavigation)
                .HasForeignKey(d => d.ReportsTo)
                .HasConstraintName("FK__Employees__Repor__4222D4EF");
        });

        modelBuilder.Entity<EmployeeInfo>(entity =>
        {
            entity.HasKey(e => e.InfoId).HasName("PK__Employee__4DEC9D9A490C0328");

            entity.ToTable("EmployeeInfo");

            entity.Property(e => e.InfoId).HasColumnName("InfoID");
            entity.Property(e => e.BankAccountNumber).HasMaxLength(50);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.EmergencyContact).HasMaxLength(200);
            entity.Property(e => e.EmergencyPhone).HasMaxLength(20);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NrcNumber)
                .HasMaxLength(50)
                .HasColumnName("NRC_Number");
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeInfos)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeI__Emplo__76619304");
        });

        modelBuilder.Entity<FormQuestion>(entity =>
        {
            entity.ToTable("FormQuestions");

            entity.HasKey(e => new { e.FormId, e.QuestionId });

            entity.Property(e => e.FormId).HasColumnName("FormID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.SortOrder).HasColumnName("SortOrder"); 

            entity.HasOne(d => d.Form)
                .WithMany(p => p.FormQuestions)
                .HasForeignKey(d => d.FormId)
                .HasConstraintName("FK__FormQuest__FormI__3E1D39E1");

            entity.HasOne(d => d.Question)
                .WithMany(p => p.FormQuestions)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__FormQuest__Quest__3F115E1A");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Levels__09F03C06F2229DE0");

            entity.Property(e => e.LevelId)
                .HasMaxLength(10)
                .HasColumnName("LevelID");
            entity.Property(e => e.LevelName).HasMaxLength(100);
        });

        modelBuilder.Entity<MeetingNote>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PK__MeetingN__EACE357F07E7BC18");

            entity.Property(e => e.NoteId).HasColumnName("NoteID");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MeetingId).HasColumnName("MeetingID");
            entity.Property(e => e.NoteType).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Author).WithMany(p => p.MeetingNotes)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MeetingNo__Autho__37703C52");

            entity.HasOne(d => d.Meeting).WithMany(p => p.MeetingNotes)
                .HasForeignKey(d => d.MeetingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MeetingNo__Meeti__367C1819");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32DAB18F79");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.RelatedEntityId).HasColumnName("RelatedEntityID");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__0A9D95DB");
        });

        modelBuilder.Entity<OneOnOneMeeting>(entity =>
        {
            entity.HasKey(e => e.MeetingId).HasName("PK__OneOnOne__E9F9E9AC5FD2CB57");

            entity.Property(e => e.MeetingId).HasColumnName("MeetingID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.MeetingStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Scheduled");
            entity.Property(e => e.MeetingSummary).HasMaxLength(1000);
            entity.Property(e => e.ParentMeetingId).HasColumnName("ParentMeetingID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Employee).WithMany(p => p.OneOnOneMeetingEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OneOnOneM__Emplo__02FC7413");

            entity.HasOne(d => d.Manager).WithMany(p => p.OneOnOneMeetingManagers)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OneOnOneM__Manag__03F0984C");

            entity.HasOne(d => d.ParentMeeting).WithMany(p => p.InverseParentMeeting)
                .HasForeignKey(d => d.ParentMeetingId)
                .HasConstraintName("FK__OneOnOneM__Paren__05D8E0BE");
        });

        modelBuilder.Entity<PerformanceEvaluation>(entity =>
        {
            entity.HasKey(e => e.EvalId).HasName("PK__Performa__C1A298AD4C2031D7");

            entity.Property(e => e.EvalId).HasColumnName("EvalID");
            entity.Property(e => e.CycleId).HasColumnName("CycleID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Status).HasColumnName("Status").HasConversion<int>().HasDefaultValue(PerformanceEvaluationStatus.Draft);

            entity.Property(e => e.FormId).HasColumnName("FormID");

            entity.Property(e => e.FinalRatingScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsFinalized).HasDefaultValue(false);

            entity.HasOne(d => d.Form)
                .WithMany(p => p.PerformanceEvaluations)
                .HasForeignKey(d => d.FormId)
                .HasConstraintName("FK_PerformanceEvaluation_ApplicationForm");

            entity.HasOne(d => d.Cycle).WithMany(p => p.PerformanceEvaluations)
                .HasForeignKey(d => d.CycleId)
                .HasConstraintName("FK__Performan__Cycle__6383C8BA");

            entity.HasOne(d => d.Employee).WithMany(p => p.PerformanceEvaluations)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Performan__Emplo__628FA481");
        });

        modelBuilder.Entity<PerformanceOutcome>(entity =>
        {
            entity.HasKey(e => e.OutcomeId).HasName("PK__Performa__113E6AFCA751128E");

            entity.Property(e => e.OutcomeId).HasColumnName("OutcomeID");
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.CycleId).HasColumnName("CycleID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EvalId).HasColumnName("EvalID");
            entity.Property(e => e.NewLevelId)
                .HasMaxLength(10)
                .HasColumnName("NewLevelID");
            entity.Property(e => e.NewPositionId).HasColumnName("NewPositionID");
            entity.Property(e => e.OldLevelId)
                .HasMaxLength(10)
                .HasColumnName("OldLevelID");
            entity.Property(e => e.OldPositionId).HasColumnName("OldPositionID");
            entity.Property(e => e.RecommendationType).HasMaxLength(50);

            entity.HasOne(d => d.Cycle).WithMany(p => p.PerformanceOutcomes)
                .HasForeignKey(d => d.CycleId)
                .HasConstraintName("FK__Performan__Cycle__2EDAF651");

            entity.HasOne(d => d.Employee).WithMany(p => p.PerformanceOutcomes)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Performan__Emplo__2DE6D218");

            entity.HasOne(d => d.Evaluation).WithMany(p => p.PerformanceOutcomes)
                .HasForeignKey(d => d.EvalId)
                .HasConstraintName("FK_PerformanceOutcome_PerformanceEvaluation");

            entity.HasOne(d => d.NewLevel).WithMany(p => p.PerformanceOutcomeNewLevels)
                .HasForeignKey(d => d.NewLevelId)
                .HasConstraintName("FK__Performan__NewLe__32AB8735");

            entity.HasOne(d => d.NewPosition).WithMany(p => p.PerformanceOutcomeNewPositions)
                .HasForeignKey(d => d.NewPositionId)
                .HasConstraintName("FK__Performan__NewPo__30C33EC3");

            entity.HasOne(d => d.OldLevel).WithMany(p => p.PerformanceOutcomeOldLevels)
                .HasForeignKey(d => d.OldLevelId)
                .HasConstraintName("FK__Performan__OldLe__31B762FC");

            entity.HasOne(d => d.OldPosition).WithMany(p => p.PerformanceOutcomeOldPositions)
                .HasForeignKey(d => d.OldPositionId)
                .HasConstraintName("FK__Performan__OldPo__2FCF1A8A");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB0F3E29F710");

            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.PermissionCode).HasMaxLength(100);
        });

        modelBuilder.Entity<PipMeeting>(entity =>
        {
            entity.HasKey(e => e.PipMeetingId).HasName("PK__PIP_Meet__0B4AA24FE72BDF9E");

            entity.ToTable("PIP_Meetings");

            entity.Property(e => e.PipMeetingId).HasColumnName("PIP_MeetingID");
            entity.Property(e => e.Pipid).HasColumnName("PIPID");
            entity.Property(e => e.ProgressStatus).HasMaxLength(50);

            entity.HasOne(d => d.Pip).WithMany(p => p.PipMeetings)
                .HasForeignKey(d => d.Pipid)
                .HasConstraintName("FK__PIP_Meeti__PIPID__208CD6FA");
        });

        modelBuilder.Entity<PipObjective>(entity =>
        {
            entity.HasKey(e => e.ObjectiveId).HasName("PK__PIP_Obje__8C56338D0CB0AF68");

            entity.ToTable("PIP_Objectives");

            entity.Property(e => e.ObjectiveId).HasColumnName("ObjectiveID");
            entity.Property(e => e.IsAchieved).HasDefaultValue(false);
            entity.Property(e => e.Pipid).HasColumnName("PIPID");

            entity.HasOne(d => d.Pip).WithMany(p => p.PipObjectives)
                .HasForeignKey(d => d.Pipid)
                .HasConstraintName("FK__PIP_Objec__PIPID__1CBC4616");
        });

        modelBuilder.Entity<PipPlan>(entity =>
        {
            entity.HasKey(e => e.Pipid).HasName("PK__PIP_Plan__6B92E5E6667EFBC8");

            entity.ToTable("PIP_Plans");

            entity.Property(e => e.Pipid).HasColumnName("PIPID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Employee).WithMany(p => p.PipPlanEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PIP_Plans__Emplo__17036CC0");

            entity.HasOne(d => d.Manager).WithMany(p => p.PipPlanManagers)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PIP_Plans__Manag__17F790F9");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__60BB9A595C170E70");

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.LevelId)
                .HasMaxLength(10)
                .HasColumnName("LevelID");
            entity.Property(e => e.PositionTitle).HasMaxLength(100);

            entity.HasOne(d => d.Level).WithMany(p => p.Positions)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK__Positions__Level__3C69FB99");
        });

        modelBuilder.Entity<PositionPermission>(entity =>
        {
            entity.HasKey(e => new { e.PositionId, e.PermissionId });

            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Permission).WithMany(p => p.PositionPermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PositionP__Permi__51300E55");

            entity.HasOne(d => d.Position).WithMany(p => p.PositionPermissions)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PositionP__Posit__503BEA1C");
        });

        modelBuilder.Entity<RatingScale>(entity =>
        {
            entity.HasKey(e => e.RatingLevel).HasName("PK__RatingSc__78CE187BF13BD951");

            entity.Property(e => e.RatingLevel).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Label).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A88B7D902");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Teams__123AE6B909B66367");

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.TeamName).HasMaxLength(100);

            entity.HasOne(d => d.Department).WithMany(p => p.Teams)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Teams__Departmen__10566F31");

            entity.HasOne(d => d.Manager).WithMany(p => p.Teams)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Teams__ManagerID__0F624AF8");

            entity.HasMany(d => d.Employees).WithMany(p => p.TeamsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "TeamMember",
                    r => r.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TeamMembe__Emplo__14270015"),
                    l => l.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__TeamMembe__TeamI__1332DBDC"),
                    j =>
                    {
                        j.HasKey("TeamId", "EmployeeId").HasName("PK__TeamMemb__1597E3464F99DCB9");
                        j.ToTable("TeamMembers");
                        j.IndexerProperty<int>("TeamId").HasColumnName("TeamID");
                        j.IndexerProperty<int>("EmployeeId").HasColumnName("EmployeeID");
                    });
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__UserRole__AF27604F3519AECC");
            entity.ToTable("UserRoles");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACAD35B11F");

            entity.HasIndex(e => e.EmployeeId, "UQ__Users__7AD04FF02E4308F5").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

            entity.HasOne(d => d.Employee).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.EmployeeId)
                .HasConstraintName("FK__Users__EmployeeI__47DBAE45");

            entity.HasMany(d => d.Roles)
                .WithMany(p => p.Users)
                .UsingEntity<UserRole>(
                      l => l.HasOne<Role>().WithMany().HasForeignKey(ur => ur.RoleId),
                      r => r.HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId),
                      j =>
            {
                      j.HasKey(ur => new { ur.UserId, ur.RoleId });
                      j.ToTable("UserRoles");
                });
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.SystemSettingId);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
