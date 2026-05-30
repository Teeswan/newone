using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ITeamRepository _teamRepository;
    private readonly IMapper _mapper;

    public EmployeeService(IEmployeeRepository repository, ITeamRepository teamRepository, IMapper mapper)
    {
        _repository = repository;
        _teamRepository = teamRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<EmployeeDto>>(entities);
    }

    public async Task<EmployeeDetailDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return _mapper.Map<EmployeeDetailDto?>(entity);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request)
    {
        var existingCode = await _repository.GetByCodeAsync(request.EmployeeCode);
        if (existingCode != null)
        {
            throw new InvalidOperationException($"Employee Code '{request.EmployeeCode}' already exists. Please use a unique code.");
        }

        var entity = _mapper.Map<Employee>(request);
        
        if (request.TeamId.HasValue)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId.Value);
            if (team != null)
            {
                entity.TeamsNavigation.Add(team);
            }
        }

        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<EmployeeDto>(created);
    }

    public async Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        var entity = await _repository.GetByIdFromDbAsync(id);
        if (entity == null) return null;

        // Check if the new EmployeeCode already exists for another employee
        if (!string.Equals(entity.EmployeeCode, request.EmployeeCode, StringComparison.OrdinalIgnoreCase))
        {
            var existingCode = await _repository.GetByCodeAsync(request.EmployeeCode);
            if (existingCode != null)
            {
                throw new InvalidOperationException($"Employee Code '{request.EmployeeCode}' already exists. Please use a unique code.");
            }
        }

        var oldDeptId = entity.DepartmentId;
        var oldPosId = entity.PositionId;
        var oldReportsTo = entity.ReportsTo;

        _mapper.Map(request, entity);
        entity.EmployeeId = id;

        // Reset navigation properties if IDs changed to ensure EF updates the foreign keys correctly
        if (oldDeptId != entity.DepartmentId)
        {
            entity.Department = null;
        }
        if (oldPosId != entity.PositionId)
        {
            entity.Position = null;
        }
        if (oldReportsTo != entity.ReportsTo)
        {
            entity.ReportsToNavigation = null;
        }

        // Handle Team update
        entity.TeamsNavigation.Clear();
        if (request.TeamId.HasValue)
        {
            var team = await _teamRepository.GetByIdFromDbAsync(request.TeamId.Value);
            if (team != null)
            {
                entity.TeamsNavigation.Add(team);
            }
        }

        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<EmployeeDto?>(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(int departmentId)
    {
        var entities = await _repository.GetEmployeesByDepartmentAsync(departmentId);
        return _mapper.Map<IEnumerable<EmployeeDto>>(entities);
    }

    public async Task<IEnumerable<EmployeeDto>> GetByTeamAsync(int teamId)
    {
        var entities = await _repository.GetEmployeesByTeamAsync(teamId);
        return _mapper.Map<IEnumerable<EmployeeDto>>(entities);
    }

    public async Task<IEnumerable<EmployeeDto>> GetDirectReportsAsync(int managerId)
    {
        var entities = await _repository.GetDirectReportsAsync(managerId);
        return _mapper.Map<IEnumerable<EmployeeDto>>(entities);
    }

    public async Task<EmployeeDto?> GetByCodeAsync(string code)
    {
        var entity = await _repository.GetByCodeAsync(code);
        return _mapper.Map<EmployeeDto?>(entity);
    }
}

public class LevelService : ILevelService
{
    private readonly ILevelRepository _repository;
    private readonly IMapper _mapper;

    public LevelService(ILevelRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LevelDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<LevelDto>>(entities);
    }

    public async Task<LevelDto?> GetByIdAsync(string id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return _mapper.Map<LevelDto?>(entity);
    }

    public async Task<LevelDto> CreateAsync(CreateLevelRequest request)
    {
        var entity = _mapper.Map<Level>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<LevelDto>(created);
    }

    public async Task<LevelDto?> UpdateAsync(string id, UpdateLevelRequest request)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;

        _mapper.Map(request, entity);
        entity.LevelId = id;

        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<LevelDto?>(updated);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }
}

public class PositionService : IPositionService
{
    private readonly IPositionRepository _repository;
    private readonly IMapper _mapper;

    public PositionService(IPositionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PositionDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PositionDto>>(entities);
    }

    public async Task<PositionDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return _mapper.Map<PositionDto?>(entity);
    }

    public async Task<PositionDto> CreateAsync(CreatePositionRequest request)
    {
        var entity = _mapper.Map<Position>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<PositionDto>(created);
    }

    public async Task<PositionDto?> UpdateAsync(int id, UpdatePositionRequest request)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;

        var oldLevelId = entity.LevelId;

        _mapper.Map(request, entity);
        entity.PositionId = id;

        // Reset navigation property if LevelId changed to ensure EF updates the FK correctly
        if (oldLevelId != entity.LevelId)
        {
            entity.Level = null;
        }

        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<PositionDto?>(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<PositionDto>> GetByLevelAsync(string levelId)
    {
        var entities = await _repository.GetPositionsByLevelAsync(levelId);
        return _mapper.Map<IEnumerable<PositionDto>>(entities);
    }
}
