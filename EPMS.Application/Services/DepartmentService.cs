using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;
    private readonly IMapper _mapper;

    public DepartmentService(IDepartmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<DepartmentDto>>(entities);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return _mapper.Map<DepartmentDto?>(entity);
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request)
    {
        var entity = _mapper.Map<Department>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<DepartmentDto>(created);
    }

    public async Task<DepartmentDto?> UpdateAsync(int id, UpdateDepartmentRequest request)
    {
        var entity = _mapper.Map<Department>(request);
        entity.DepartmentId = id;
        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<DepartmentDto?>(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<DepartmentTreeDto>> GetTreeAsync()
    {
        var entities = await _repository.GetDepartmentTreeAsync();
        return BuildTree(entities, null, 1);
    }

    private IEnumerable<DepartmentTreeDto> BuildTree(IEnumerable<Department> allDepartments, int? parentId, int level)
    {
        var departments = allDepartments.Where(d => d.ParentDepartmentId == parentId).ToList();
        var tree = new List<DepartmentTreeDto>();

        foreach (var dept in departments)
        {
            var node = _mapper.Map<DepartmentTreeDto>(dept);
            node.Level = level;
            node.Children = BuildTree(allDepartments, dept.DepartmentId, level + 1).ToList();
            tree.Add(node);
        }

        return tree;
    }
}
