using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;

namespace EPMS.Application.Services;

public class PerformanceEvaluationService : IPerformanceEvaluationService
{
    private readonly IPerformanceEvaluationRepository _repository;
    private readonly IAppraisalResponseRepository _responseRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAppraisalCycleRepository _cycleRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IRatingScaleRepository _ratingScaleRepository;
    private readonly IMapper _mapper;

    public PerformanceEvaluationService(
        IPerformanceEvaluationRepository repository, 
        IAppraisalResponseRepository responseRepository,
        IEmployeeRepository employeeRepository,
        IAppraisalCycleRepository cycleRepository,
        IDepartmentRepository departmentRepository,
        IRatingScaleRepository ratingScaleRepository,
        IMapper mapper)
    {
        _repository = repository;
        _responseRepository = responseRepository;
        _employeeRepository = employeeRepository;
        _cycleRepository = cycleRepository;
        _departmentRepository = departmentRepository;
        _ratingScaleRepository = ratingScaleRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PerformanceEvaluationDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PerformanceEvaluationDto>>(entities);
    }

    public async Task<PerformanceEvaluationDto?> GetByIdAsync(int evalId)
    {
        var entity = await _repository.GetByIdAsync(evalId);
        return _mapper.Map<PerformanceEvaluationDto?>(entity);
    }

    public async Task<IEnumerable<PerformanceEvaluationDto>> GetByEmployeeIdAsync(int employeeId)
    {
        var entities = await _repository.GetByEmployeeIdAsync(employeeId);
        return _mapper.Map<IEnumerable<PerformanceEvaluationDto>>(entities);
    }

    public async Task<IEnumerable<PerformanceEvaluationDto>> GetByCycleIdAsync(int cycleId)
    {
        var entities = await _repository.GetByCycleIdAsync(cycleId);
        return _mapper.Map<IEnumerable<PerformanceEvaluationDto>>(entities);
    }

    public async Task<PerformanceEvaluationDto> CreateAsync(CreatePerformanceEvaluationRequest request)
    {
        var entity = _mapper.Map<PerformanceEvaluation>(request);
        var created = await _repository.CreateAsync(entity);

        if (request.Responses != null && request.Responses.Any())
        {
            foreach (var respReq in request.Responses)
            {
                var resp = _mapper.Map<AppraisalResponse>(respReq);
                resp.EvalId = created.EvalId;
                await _responseRepository.CreateAsync(resp);
            }
        }

        return _mapper.Map<PerformanceEvaluationDto>(created);
    }

    public async Task<PerformanceEvaluationDto?> UpdateAsync(int evalId, UpdatePerformanceEvaluationRequest request)
    {
        var entity = _mapper.Map<PerformanceEvaluation>(request);
        entity.EvalId = evalId;

        var updated = await _repository.UpdateAsync(entity);
        return _mapper.Map<PerformanceEvaluationDto?>(updated);
    }

    public async Task<bool> SubmitSelfAssessmentAsync(int evalId)
    {
        var evaluation = await _repository.GetByIdAsync(evalId);
        if (evaluation == null) return false;

        evaluation.Status = PerformanceEvaluationStatus.SelfSubmitted;
        await _repository.UpdateAsync(evaluation);
        return true;
    }

    public async Task<bool> DeleteAsync(int evalId)
    {
        return await _repository.DeleteAsync(evalId);
    }

    public async Task<AppraisalReportDto?> GetAppraisalReportDataAsync(int evalId)
    {
        var eval = await _repository.GetByIdAsync(evalId);
        if (eval == null) return null;

        var employee = eval.EmployeeId.HasValue ? await _employeeRepository.GetByIdAsync(eval.EmployeeId.Value) : null;
        var cycle = eval.CycleId.HasValue ? await _cycleRepository.GetByIdAsync(eval.CycleId.Value) : null;
        var dept = employee?.DepartmentId.HasValue == true ? await _departmentRepository.GetByIdAsync(employee.DepartmentId.Value) : null;
        var responses = await _responseRepository.GetByEvalIdAsync(evalId);
        
        var scales = await _ratingScaleRepository.GetAllAsync();
        var band = scales.OrderByDescending(s => s.RatingLevel)
            .FirstOrDefault(s => eval.FinalRatingScore >= s.RatingLevel);

        return new AppraisalReportDto
        {
            EmployeeName = employee?.FullName ?? "N/A",
            EmployeeCode = employee?.EmployeeCode ?? "N/A",
            DepartmentName = dept?.DepartmentName ?? "N/A",
            PositionTitle = employee?.Position?.PositionTitle ?? "N/A",
            CycleName = cycle?.CycleName ?? "N/A",
            AssessmentDate = eval.FinalizedAt ?? DateTime.Now,
            EffectiveDate = cycle != null ? new DateTime(cycle.EndDate.Year, cycle.EndDate.Month, cycle.EndDate.Day) : (DateTime?)null,
            FinalScore = eval.FinalRatingScore,
            PerformanceBand = band?.Label ?? "N/A",
            Responses = _mapper.Map<List<AppraisalResponseDto>>(responses)
        };
    }
}
