using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class AppraisalCycleService : IAppraisalCycleService
{
    private readonly IAppraisalCycleRepository _repository;
    private readonly IMapper _mapper;

    public AppraisalCycleService(IAppraisalCycleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppraisalCycleDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<AppraisalCycleDto>>(entities);
    }

    public async Task<AppraisalCycleDto?> GetByIdAsync(int cycleId)
    {
        var entity = await _repository.GetByIdAsync(cycleId);
        return _mapper.Map<AppraisalCycleDto?>(entity);
    }

    public async Task<AppraisalCycleDto> CreateAsync(CreateAppraisalCycleRequest request)
    {
        var entity = _mapper.Map<AppraisalCycle>(request);
        var created = await _repository.CreateAsync(entity);
        return _mapper.Map<AppraisalCycleDto>(created);
    }

    public async Task<AppraisalCycleDto?> UpdateAsync(int cycleId, UpdateAppraisalCycleRequest request)
    {
        var entity = _mapper.Map<AppraisalCycle>(request);
        entity.CycleId = cycleId;

        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<AppraisalCycleDto?>(updated);
    }

    public async Task<bool> DeleteAsync(int cycleId)
    {
        return await _repository.DeleteAsync(cycleId);
    }
}
