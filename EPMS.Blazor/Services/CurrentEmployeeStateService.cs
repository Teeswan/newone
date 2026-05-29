using EPMS.Shared.DTOs;

namespace EPMS.Blazor.Services
{
    public class CurrentEmployeeStateService : ICurrentEmployeeStateService
    {
        private EmployeeDetailDto? _currentEmployee;
        
        public EmployeeDetailDto? CurrentEmployee => _currentEmployee;

        public event Action? CurrentEmployeeChanged;

        public void SetCurrentEmployee(EmployeeDetailDto employee)
        {
            _currentEmployee = employee;
            CurrentEmployeeChanged?.Invoke();
        }

        public void UpdateProfilePicture(byte[] profilePictureBytes)
        {
            if (_currentEmployee != null)
            {
                // Create a shallow clone to ensure proper UI updates
                var updatedEmployee = new EmployeeDetailDto
                {
                    EmployeeId = _currentEmployee.EmployeeId,
                    EmployeeCode = _currentEmployee.EmployeeCode,
                    FullName = _currentEmployee.FullName,
                    Email = _currentEmployee.Email,
                    PositionId = _currentEmployee.PositionId,
                    PositionTitle = _currentEmployee.PositionTitle,
                    DepartmentId = _currentEmployee.DepartmentId,
                    DepartmentName = _currentEmployee.DepartmentName,
                    TeamId = _currentEmployee.TeamId,
                    TeamName = _currentEmployee.TeamName,
                    ReportsTo = _currentEmployee.ReportsTo,
                    JoinDate = _currentEmployee.JoinDate,
                    Phone = _currentEmployee.Phone,
                    Address = _currentEmployee.Address,
                    EmploymentStatus = _currentEmployee.EmploymentStatus,
                    NrcNumber = _currentEmployee.NrcNumber,
                    DateOfBirth = _currentEmployee.DateOfBirth,
                    Gender = _currentEmployee.Gender,
                    EmergencyContact = _currentEmployee.EmergencyContact,
                    EmergencyPhone = _currentEmployee.EmergencyPhone,
                    BankName = _currentEmployee.BankName,
                    BankAccountNumber = _currentEmployee.BankAccountNumber,
                    CurrentSalary = _currentEmployee.CurrentSalary,
                    IsActive = _currentEmployee.IsActive,
                    ProfilePicture = profilePictureBytes
                };
                _currentEmployee = updatedEmployee;
                CurrentEmployeeChanged?.Invoke();
            }
        }

        public void ClearCurrentEmployee()
        {
            _currentEmployee = null;
            CurrentEmployeeChanged?.Invoke();
        }
    }
}