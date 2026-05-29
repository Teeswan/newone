using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services
{
    public interface ICurrentEmployeeStateService
    {
        EmployeeDetailDto? CurrentEmployee { get; }
        event Action? CurrentEmployeeChanged;
        void SetCurrentEmployee(EmployeeDetailDto employee);
        void UpdateProfilePicture(byte[] profilePictureBytes);
        void ClearCurrentEmployee();
    }
}