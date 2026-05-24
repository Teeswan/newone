using System.ComponentModel.DataAnnotations;

namespace EPMS.Shared.Requests;

public class ChangePasswordRequest
{
    [Required]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "New Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string NewPassword { get; set; } = string.Empty;
}
