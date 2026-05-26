using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPMS.Application.Services
{
    public class KpiService : IKpiService
    {
        private readonly IKpiRepository _repository;
        private readonly IMapper _mapper;

        public KpiService(IKpiRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<KpiDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<KpiDto>>(entities);
        }

        public async Task<KpiDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<KpiDto?>(entity);
        }

        public async Task<KpiDto> CreateAsync(KpiRequest request, int? createdByEmployeeId)
        {
            var entity = Kpi.Create(
                request.KpiName,
                request.Category,
                request.Unit,
                request.Direction,
                createdByEmployeeId);
            
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<KpiDto>(created);
        }

        public async Task<KpiDto?> UpdateAsync(int id, KpiRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Update(
                request.KpiName,
                request.Category,
                request.Unit,
                request.Direction);
            
            await _repository.UpdateAsync(entity);
            return _mapper.Map<KpiDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }

    public class DepartmentKpiService : IDepartmentKpiService
    {
        private readonly IDepartmentKpiRepository _repository;
        private readonly IMapper _mapper;

        public DepartmentKpiService(IDepartmentKpiRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentKpiDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<DepartmentKpiDto>>(entities);
        }

        public async Task<DepartmentKpiDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<DepartmentKpiDto?>(entity);
        }

        public async Task<IEnumerable<DepartmentKpiDto>> GetByDepartmentIdAsync(int departmentId, int cycleId)
        {
            var entities = await _repository.GetByDepartmentIdAsync(departmentId, cycleId);
            return _mapper.Map<IEnumerable<DepartmentKpiDto>>(entities);
        }

        public async Task<DepartmentKpiDto> CreateAsync(DepartmentKpiRequest request)
        {
            var existing = await _repository.GetByDepartmentIdAsync(request.DepartmentId, request.CycleId);
            if (existing.Sum(k => k.Weight) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this department and cycle cannot exceed 100%.");

            var entity = DepartmentKpi.Create(
                request.DepartmentId,
                request.CycleId,
                request.KpiId,
                request.DepartmentTarget,
                request.Weight);
            
            var created = await _repository.CreateAsync(entity);
            var result = await _repository.GetByIdAsync(created.DeptKpiId);
            return _mapper.Map<DepartmentKpiDto>(result);
        }

        public async Task<DepartmentKpiDto?> UpdateAsync(int id, DepartmentKpiRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            var otherKpis = (await _repository.GetByDepartmentIdAsync(entity.DepartmentId, entity.CycleId))
                .Where(k => k.DeptKpiId != id);
            
            if (otherKpis.Sum(k => k.Weight) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this department and cycle cannot exceed 100%.");

            entity.Update(request.DepartmentTarget, request.Weight);
            await _repository.UpdateAsync(entity);
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<DepartmentKpiDto>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }

    public class TeamKpiService : ITeamKpiService
    {
        private readonly ITeamKpiRepository _repository;
        private readonly IMapper _mapper;

        public TeamKpiService(ITeamKpiRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TeamKpiDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TeamKpiDto>>(entities);
        }

        public async Task<TeamKpiDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<TeamKpiDto?>(entity);
        }

        public async Task<IEnumerable<TeamKpiDto>> GetByTeamIdAsync(int teamId)
        {
            var entities = await _repository.GetByTeamIdAsync(teamId);
            return _mapper.Map<IEnumerable<TeamKpiDto>>(entities);
        }

        public async Task<TeamKpiDto> CreateAsync(TeamKpiRequest request)
        {
            var existing = await _repository.GetByTeamIdAsync(request.TeamId);
            if (existing.Sum(k => k.Weight) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this team cannot exceed 100%.");

            var entity = TeamKpi.Create(
                request.TeamId,
                request.DeptKpiId,
                request.TeamTarget,
                request.Weight);
            
            var created = await _repository.CreateAsync(entity);
            var result = await _repository.GetByIdAsync(created.TeamKpiId);
            return _mapper.Map<TeamKpiDto>(result);
        }

        public async Task<TeamKpiDto?> UpdateAsync(int id, TeamKpiRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            var otherKpis = (await _repository.GetByTeamIdAsync(entity.TeamId))
                .Where(k => k.TeamKpiId != id);
            
            if (otherKpis.Sum(k => k.Weight) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this team cannot exceed 100%.");

            entity.Update(request.TeamTarget, request.Weight);
            await _repository.UpdateAsync(entity);
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<TeamKpiDto>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }

    public class EmployeeKpiService : IEmployeeKpiService
    {
        private readonly IEmployeeKpiRepository _repository;
        private readonly IMapper _mapper;

        public EmployeeKpiService(IEmployeeKpiRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeKpiDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeKpiDto>>(entities);
        }

        public async Task<EmployeeKpiDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<EmployeeKpiDto?>(entity);
        }

        public async Task<IEnumerable<EmployeeKpiDto>> GetByEmployeeIdAsync(int employeeId)
        {
            var entities = await _repository.GetByEmployeeIdAsync(employeeId);
            return _mapper.Map<IEnumerable<EmployeeKpiDto>>(entities);
        }

        public async Task<EmployeeKpiDto> CreateAsync(EmployeeKpiRequest request)
        {
            var existing = await _repository.GetByEmployeeIdAsync(request.EmployeeId);
            if (existing.Sum(k => k.Weight) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this employee cannot exceed 100%.");

            var entity = EmployeeKpi.Create(
                request.EmployeeId,
                request.TeamKpiId,
                request.EmployeeTarget,
                request.Weight);
            
            var created = await _repository.CreateAsync(entity);
            var result = await _repository.GetByIdAsync(created.EmployeeKpiId);
            return _mapper.Map<EmployeeKpiDto>(result);
        }

        public async Task<EmployeeKpiDto?> UpdateAsync(int id, EmployeeKpiRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            var otherKpis = (await _repository.GetByEmployeeIdAsync(entity.EmployeeId))
                .Where(k => k.EmployeeKpiId != id);
            
            if (otherKpis.Sum(k => k.Weight) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this employee cannot exceed 100%.");

            entity.Update(request.EmployeeTarget, request.Weight);
            await _repository.UpdateAsync(entity);
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<EmployeeKpiDto>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
