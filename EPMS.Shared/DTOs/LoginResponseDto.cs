namespace EPMS.Shared.DTOs;

public class LoginResponseDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsFirstLogin { get; set; }
    public string Token { get; set; } = string.Empty;
}

