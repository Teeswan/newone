using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAppraisalCycleRepository _cycleRepository;
        private readonly IMapper _mapper;

        public DepartmentKpiService(
            IDepartmentKpiRepository repository, 
            IAppraisalCycleRepository cycleRepository,
            IMapper mapper)
        {
            _repository = repository;
            _cycleRepository = cycleRepository;
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
            if (existing.Sum(k => k.Weight ?? 0) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this department and cycle cannot exceed 100%.");

            var entity = DepartmentKpi.Create(
                request.DepartmentId,
                request.CycleId,
                request.KpiId,
                request.DepartmentTarget,
                request.Weight,
                request.ActualValue);
            
            var created = await _repository.CreateAsync(entity);
            var result = await _repository.GetByIdAsync(created.DeptKpiId);
            return _mapper.Map<DepartmentKpiDto>(result);
        }

        public async Task<DepartmentKpiDto?> UpdateAsync(int id, DepartmentKpiRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            var otherKpis = (await _repository.GetByDepartmentIdAsync(entity.DepartmentId ?? 0, entity.CycleId ?? 0))
                .Where(k => k.DeptKpiId != id);
            
            if (otherKpis.Sum(k => k.Weight ?? 0) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this department and cycle cannot exceed 100%.");

            entity.Update(request.DepartmentTarget, request.Weight, request.ActualValue);
            await _repository.UpdateAsync(entity);
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<DepartmentKpiDto>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<DepartmentKpiDto>> CalculateForDepartmentAsync(int departmentId)
        {
            var cycles = await _cycleRepository.GetAllAsync();
            var activeCycle = cycles.FirstOrDefault(c => c.CycleStatus == "Active") 
                            ?? cycles.OrderByDescending(c => c.CycleId).FirstOrDefault();
            
            if (activeCycle == null) throw new InvalidOperationException("No active appraisal cycle found.");

            var currentCycleKpis = (await _repository.GetByDepartmentIdAsync(departmentId, activeCycle.CycleId)).ToList();

            foreach (var dkpi in currentCycleKpis)
            {
                // Aggregate weighted scores from all teams in this department for this specific KPI
                if (dkpi.TeamKpis.Any())
                {
                    var aggregatedActual = dkpi.TeamKpis.Average(t => t.WeightedScore) ?? 0;
                    dkpi.Update(dkpi.DepartmentTarget ?? 0, dkpi.Weight ?? 0, aggregatedActual);
                    await _repository.UpdateAsync(dkpi);
                }
            }

            return _mapper.Map<IEnumerable<DepartmentKpiDto>>(currentCycleKpis);
        }
    }

    public class TeamKpiService : ITeamKpiService
    {
        private readonly ITeamKpiRepository _repository;
        private readonly IDepartmentKpiRepository _deptKpiRepository;
        private readonly IEmployeeKpiRepository _employeeKpiRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IAppraisalCycleRepository _cycleRepository;
        private readonly IMapper _mapper;

        public TeamKpiService(
            ITeamKpiRepository repository,
            IDepartmentKpiRepository deptKpiRepository,
            IEmployeeKpiRepository employeeKpiRepository,
            ITeamRepository teamRepository,
            IAppraisalCycleRepository cycleRepository,
            IMapper mapper)
        {
            _repository = repository;
            _deptKpiRepository = deptKpiRepository;
            _employeeKpiRepository = employeeKpiRepository;
            _teamRepository = teamRepository;
            _cycleRepository = cycleRepository;
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
            if (existing.Sum(k => k.Weight ?? 0) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this team cannot exceed 100%.");

            var entity = TeamKpi.Create(
                request.TeamId,
                request.DeptKpiId,
                request.TeamTarget,
                request.Weight,
                request.ActualValue);
            
            var created = await _repository.CreateAsync(entity);
            var result = await _repository.GetByIdAsync(created.TeamKpiId);
            return _mapper.Map<TeamKpiDto>(result);
        }

        public async Task<TeamKpiDto?> UpdateAsync(int id, TeamKpiRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            var otherKpis = (await _repository.GetByTeamIdAsync(entity.TeamId ?? 0))
                .Where(k => k.TeamKpiId != id);
            
            if (otherKpis.Sum(k => k.Weight ?? 0) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this team cannot exceed 100%.");

            entity.Update(request.TeamTarget, request.Weight, request.ActualValue);
            if (!request.IsActive) entity.Deactivate(); else entity.Activate();

            await _repository.UpdateAsync(entity);
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<TeamKpiDto>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TeamKpiDto>> CalculateForTeamAsync(int teamId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null) throw new InvalidOperationException("Team not found.");

            var cycles = await _cycleRepository.GetAllAsync();
            var activeCycle = cycles.FirstOrDefault(c => c.CycleStatus == "Active") 
                            ?? cycles.OrderByDescending(c => c.CycleId).FirstOrDefault();
            
            if (activeCycle == null) throw new InvalidOperationException("No active appraisal cycle found.");

            // 1. Get or Assign Team KPIs
            var existingKpis = await _repository.GetByTeamIdAsync(teamId);
            var currentCycleKpis = existingKpis.Where(k => k.DepartmentKpi?.CycleId == activeCycle.CycleId).ToList();

            if (!currentCycleKpis.Any())
            {
                if (team.DepartmentId == null) throw new InvalidOperationException("Team is not assigned to a department.");

                var deptKpis = await _deptKpiRepository.GetByDepartmentIdAsync(team.DepartmentId.Value, activeCycle.CycleId);
                
                foreach (var dk in deptKpis)
                {
                    var newKpi = TeamKpi.Create(
                        teamId,
                        dk.DeptKpiId,
                        dk.DepartmentTarget ?? 0,
                        dk.Weight ?? 0);
                    await _repository.CreateAsync(newKpi);
                }
                
                existingKpis = await _repository.GetByTeamIdAsync(teamId);
                currentCycleKpis = existingKpis.Where(k => k.DepartmentKpi?.CycleId == activeCycle.CycleId).ToList();
            }

            // 2. Perform Calculation (Average of Employee KPIs)
            foreach (var tkpi in currentCycleKpis)
            {
                var employeeKpis = await _employeeKpiRepository.GetByTeamKpiIdAsync(tkpi.TeamKpiId);
                if (employeeKpis.Any())
                {
                    // Calculate aggregated actual value (average)
                    var aggregatedActual = employeeKpis.Average(e => e.ActualValue) ?? 0;
                    
                    // Update the Team KPI with new actual value
                    tkpi.Update(tkpi.TeamTarget ?? 0, tkpi.Weight ?? 0, aggregatedActual);
                    await _repository.UpdateAsync(tkpi);
                }
            }

            return _mapper.Map<IEnumerable<TeamKpiDto>>(currentCycleKpis);
        }
    }

    public class EmployeeKpiService : IEmployeeKpiService
    {
        private readonly IEmployeeKpiRepository _repository;
        private readonly IPositionKpiRepository _positionKpiRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAppraisalCycleRepository _cycleRepository;
        private readonly IMapper _mapper;

        public EmployeeKpiService(
            IEmployeeKpiRepository repository, 
            IPositionKpiRepository positionKpiRepository,
            IEmployeeRepository employeeRepository,
            IAppraisalCycleRepository cycleRepository,
            IMapper mapper)
        {
            _repository = repository;
            _positionKpiRepository = positionKpiRepository;
            _employeeRepository = employeeRepository;
            _cycleRepository = cycleRepository;
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
            if (existing.Where(k => k.CycleId == request.CycleId).Sum(k => k.Weight ?? 0) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this employee in this cycle cannot exceed 100%.");

            var entity = new EmployeeKpi
            {
                EmployeeId = request.EmployeeId,
                CycleId = request.CycleId,
                KpiId = request.KpiId,
                PositionKpiId = request.PositionKpiId,
                TeamKpiId = request.TeamKpiId,
                TargetValue = request.TargetValue,
                Weight = request.Weight,
                IsActive = request.IsActive,
                ActualValue = request.ActualValue,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(entity);
            var result = await _repository.GetByIdAsync(created.EmployeeKpiId);
            return _mapper.Map<EmployeeKpiDto>(result);
        }

        public async Task<IEnumerable<EmployeeKpiDto>> CreateBulkAsync(BulkEmployeeKpiRequest request)
        {
            var results = new List<EmployeeKpi>();
            foreach (var employeeId in request.EmployeeIds)
            {
                var existing = await _repository.GetByEmployeeIdAsync(employeeId);
                if (existing.Where(k => k.CycleId == request.CycleId).Sum(k => k.Weight ?? 0) + request.Weight > 100)
                    continue; 

                var entity = new EmployeeKpi
                {
                    EmployeeId = employeeId,
                    CycleId = request.CycleId,
                    KpiId = request.KpiId,
                    PositionKpiId = request.PositionKpiId,
                    TeamKpiId = request.TeamKpiId,
                    TargetValue = request.TargetValue,
                    Weight = request.Weight,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _repository.CreateAsync(entity);
                results.Add(await _repository.GetByIdAsync(created.EmployeeKpiId));
            }

            return _mapper.Map<IEnumerable<EmployeeKpiDto>>(results);
        }

        public async Task<EmployeeKpiDto?> UpdateAsync(int id, EmployeeKpiRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            var otherKpis = (await _repository.GetByEmployeeIdAsync(entity.EmployeeId))
                .Where(k => k.EmployeeKpiId != id && k.CycleId == entity.CycleId);

            if (otherKpis.Sum(k => k.Weight ?? 0) + request.Weight > 100)
                throw new InvalidOperationException("Total weight for this employee in this cycle cannot exceed 100%.");

            entity.Weight = request.Weight;
            entity.TargetValue = request.TargetValue;
            entity.ActualValue = request.ActualValue;
            entity.IsActive = request.IsActive;

            await _repository.UpdateAsync(entity);
            var result = await _repository.GetByIdAsync(id);
            return _mapper.Map<EmployeeKpiDto>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<EmployeeKpiDto>> CalculateForEmployeeAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) throw new InvalidOperationException("Employee not found.");

            var cycles = await _cycleRepository.GetAllAsync();
            var activeCycle = cycles.FirstOrDefault(c => c.CycleStatus == "Active") 
                            ?? cycles.OrderByDescending(c => c.CycleId).FirstOrDefault();
            
            if (activeCycle == null) throw new InvalidOperationException("No active appraisal cycle found.");

            var existingKpis = await _repository.GetByEmployeeIdAsync(employeeId);
            var currentCycleKpis = existingKpis.Where(k => k.CycleId == activeCycle.CycleId).ToList();

            if (!currentCycleKpis.Any())
            {
                // No EmployeeKpis, fallback to PositionKpis
                var positionKpis = await _positionKpiRepository.GetListByPositionAsync(employee.PositionId);
                
                foreach (var pk in positionKpis)
                {
                    var newKpi = new EmployeeKpi
                    {
                        EmployeeId = employeeId,
                        CycleId = activeCycle.CycleId,
                        KpiId = pk.KpiId,
                        PositionKpiId = pk.PositionKpiId,
                        TargetValue = pk.TargetValue,
                        Weight = pk.DefaultWeightPercent,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _repository.CreateAsync(newKpi);
                }
                
                currentCycleKpis = (await _repository.GetByEmployeeIdAsync(employeeId))
                                    .Where(k => k.CycleId == activeCycle.CycleId).ToList();
            }

            return _mapper.Map<IEnumerable<EmployeeKpiDto>>(currentCycleKpis);
        }
    }
}
