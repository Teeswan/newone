using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _repository;
    private readonly IMapper _mapper;

    public TeamService(ITeamRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TeamDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TeamDto>>(entities);
    }

    public async Task<TeamDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return _mapper.Map<TeamDto?>(entity);
    }

    public async Task<TeamDto> CreateAsync(CreateTeamRequest request)
    {
        var entity = _mapper.Map<Team>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<TeamDto>(created);
    }

    public async Task<TeamDto?> UpdateAsync(int id, UpdateTeamRequest request)
    {
        var existingEntity = await _repository.GetByIdAsync(id);
        if (existingEntity == null) return null;

        var oldDeptId = existingEntity.DepartmentId;
        var oldManagerId = existingEntity.ManagerId;

        _mapper.Map(request, existingEntity);
        existingEntity.TeamId = id;

        // Reset navigation properties if IDs changed to ensure EF updates the foreign keys correctly
        if (oldDeptId != existingEntity.DepartmentId)
        {
            existingEntity.Department = null;
        }
        if (oldManagerId != existingEntity.ManagerId)
        {
            existingEntity.Manager = null;
        }
        
        var updated = await _repository.UpdateAsync(existingEntity);
        return _mapper.Map<TeamDto?>(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TeamDetailDto>> GetByDepartmentAsync(int departmentId)
    {
        var entities = await _repository.GetTeamsByDepartmentAsync(departmentId);
        // Mapping from dynamic result of SP
        return entities.Select(e => new TeamDetailDto
        {
            TeamId = (int)e.TeamId,
            TeamName = (string)e.TeamName,
            ManagerId = (int?)e.ManagerId,
            ManagerName = (string)e.ManagerName,
            DepartmentId = (int?)e.DepartmentId,
            DepartmentName = (string)e.DepartmentName,
            MemberCount = (int)e.MemberCount
        });
    }
}
