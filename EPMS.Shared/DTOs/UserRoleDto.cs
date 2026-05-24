namespace EPMS.Shared.DTOs
{
    public class UserRoleDto
    {
        public int UserID { get; set; }
        public int EmployeeID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsActive { get; set; } = true;
    }
}
