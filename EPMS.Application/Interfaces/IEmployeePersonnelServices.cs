using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDetailDto?> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request);
    Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(int departmentId);
    Task<IEnumerable<EmployeeDto>> GetDirectReportsAsync(int managerId);
    Task<EmployeeDto?> GetByCodeAsync(string code);
}

public interface ILevelService
{
    Task<IEnumerable<LevelDto>> GetAllAsync();
    Task<LevelDto?> GetByIdAsync(string id);
    Task<LevelDto> CreateAsync(CreateLevelRequest request);
    Task<LevelDto?> UpdateAsync(string id, UpdateLevelRequest request);
    Task<bool> DeleteAsync(string id);
}

public interface IPositionService
{
    Task<IEnumerable<PositionDto>> GetAllAsync();
    Task<PositionDto?> GetByIdAsync(int id);
    Task<PositionDto> CreateAsync(CreatePositionRequest request);
    Task<PositionDto?> UpdateAsync(int id, UpdatePositionRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<PositionDto>> GetByLevelAsync(string levelId);
}
