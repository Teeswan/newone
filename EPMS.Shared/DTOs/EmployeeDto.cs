namespace EPMS.Shared.DTOs;

public class EmployeeDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int? PositionId { get; set; }
    public string? PositionTitle { get; set; }
    public int? ReportsTo { get; set; }
    public string? ManagerName { get; set; }
    public string? TeamNames { get; set; }
    public bool? IsActive { get; set; }
    public string? Phone { get; set; }
    public DateTime? JoinDate { get; set; }
    public string? EmploymentStatus { get; set; }
}

public class EmployeeDetailDto : EmployeeDto
{
    public decimal? CurrentSalary { get; set; }
    public string? NrcNumber { get; set; }
    public List<int> TeamIds { get; set; } = new();
    public bool? PromotionEligibility { get; set; }
    public DateTime? LastPromotionDate { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public byte[]? ProfilePicture { get; set; }
}
