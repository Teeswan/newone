using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EPMS.Domain.Interfaces;

namespace EPMS.Domain.Entities;

public partial class Employee : ISoftDelete
{
    public int EmployeeId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Username { get; set; }

    public string? PasswordHash { get; set; }

    public bool IsFirstLogin { get; set; }

    public int? DepartmentId { get; set; }

    public int? PositionId { get; set; }

    public int? ReportsTo { get; set; }

    public DateTime? JoinDate { get; set; }

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public decimal? CurrentSalary { get; set; }

    [Column("NRC_Number")]
    public string? NrcNumber { get; set; }

    public bool? PromotionEligibility { get; set; }

    public DateTime? LastPromotionDate { get; set; }

    public string? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? EmergencyContact { get; set; }

    public string? EmergencyPhone { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? EmploymentStatus { get; set; }

    public byte[]? ProfilePicture { get; set; }

    public virtual ICollection<AppraisalResponse> AppraisalResponses { get; set; } = new List<AppraisalResponse>();

    public virtual Department? Department { get; set; }

    public virtual ICollection<EmployeeInfo> EmployeeInfos { get; set; } = new List<EmployeeInfo>();

    public virtual ICollection<Employee> InverseReportsToNavigation { get; set; } = new List<Employee>();

    public virtual ICollection<MeetingNote> MeetingNotes { get; set; } = new List<MeetingNote>();

    public virtual ICollection<OneOnOneMeeting> OneOnOneMeetingEmployees { get; set; } = new List<OneOnOneMeeting>();

    public virtual ICollection<OneOnOneMeeting> OneOnOneMeetingManagers { get; set; } = new List<OneOnOneMeeting>();

    public virtual ICollection<PerformanceEvaluation> PerformanceEvaluations { get; set; } = new List<PerformanceEvaluation>();

    public virtual ICollection<PerformanceOutcome> PerformanceOutcomes { get; set; } = new List<PerformanceOutcome>();

    public virtual ICollection<PipPlan> PipPlanEmployees { get; set; } = new List<PipPlan>();

    public virtual ICollection<PipPlan> PipPlanManagers { get; set; } = new List<PipPlan>();

    public virtual Position? Position { get; set; }

    public virtual Employee? ReportsToNavigation { get; set; }

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual User? User { get; set; }

    public virtual ICollection<Team> TeamsNavigation { get; set; } = new List<Team>();
}
