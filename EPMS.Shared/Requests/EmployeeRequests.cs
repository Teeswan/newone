namespace EPMS.Shared.Requests;

public class CreateEmployeeRequest
{
    public string EmployeeCode { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public int? DepartmentId { get; set; }
    public int? PositionId { get; set; }
    public int? ReportsTo { get; set; }
    public DateTime? JoinDate { get; set; }
    public decimal? CurrentSalary { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? EmploymentStatus { get; set; }
    public string? NrcNumber { get; set; }
}

public class UpdateEmployeeRequest : CreateEmployeeRequest
{
    public bool? IsActive { get; set; }
    public string? Gender { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
}

public class CreateLevelRequest
{
    public string LevelId { get; set; } = null!;
    public string LevelName { get; set; } = null!;
    public string? LevelDescription { get; set; }
}

public class UpdateLevelRequest
{
    public string LevelName { get; set; } = null!;
    public string? LevelDescription { get; set; }
}

public class CreatePositionRequest
{
    public string PositionTitle { get; set; } = null!;
    public string? LevelId { get; set; }
}

public class UpdatePositionRequest : CreatePositionRequest
{
}
