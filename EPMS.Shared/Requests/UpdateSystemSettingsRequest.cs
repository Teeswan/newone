using System.ComponentModel.DataAnnotations;

namespace EPMS.Shared.Requests;

public class UpdateSystemSettingsRequest
{
    [Required(ErrorMessage = "New Default Password is required")]
    public string NewDefaultPassword { get; set; } = string.Empty;
}
