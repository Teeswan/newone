namespace EPMS.Shared.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
