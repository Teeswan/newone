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
    private readonly IPerformanceOutcomeRepository _outcomeRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAppraisalCycleRepository _cycleRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IRatingScaleRepository _ratingScaleRepository;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public PerformanceEvaluationService(
        IPerformanceEvaluationRepository repository, 
        IAppraisalResponseRepository responseRepository,
        IPerformanceOutcomeRepository outcomeRepository,
        IEmployeeRepository employeeRepository,
        IAppraisalCycleRepository cycleRepository,
        IDepartmentRepository departmentRepository,
        IRatingScaleRepository ratingScaleRepository,
        INotificationService notificationService,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _repository = repository;
        _responseRepository = responseRepository;
        _outcomeRepository = outcomeRepository;
        _employeeRepository = employeeRepository;
        _cycleRepository = cycleRepository;
        _departmentRepository = departmentRepository;
        _ratingScaleRepository = ratingScaleRepository;
        _notificationService = notificationService;
        _userRepository = userRepository;
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

            // Re-calculate totals and FinalRatingScore based on created responses
            var allResponses = await _responseRepository.GetByEvalIdAsync(created.EvalId);
            var respList = allResponses.ToList();
            if (respList.Any())
            {
                var selfRatings = respList.Where(r => r.RespondentRole == "Self" && r.RatingValue.HasValue).ToList();
                var otherRatings = respList.Where(r => r.RespondentRole != "Self" && r.RatingValue.HasValue).ToList();

                var selfTotal = selfRatings.Sum(r => r.RatingValue ?? 0);
                var managerTotal = respList.Where(r => r.RespondentRole == "Manager").Sum(r => r.RatingValue ?? 0);

                var activeRatings = otherRatings.Any() ? otherRatings : selfRatings;
                var totalPoints = activeRatings.Sum(r => r.RatingValue ?? 0);
                var questionsCount = activeRatings.Count();

                created.SelfRating = selfTotal;
                created.ManagerRating = managerTotal;
                if (questionsCount > 0)
                {
                    created.FinalRatingScore = (decimal)totalPoints / (questionsCount * 5) * 100;
                }
                await _repository.UpdateAsync(created);
            }
        }

        // Send notifications
        if (created.EmployeeId.HasValue)
        {
            var employeeUser = await _userRepository.GetByEmployeeIdAsync(created.EmployeeId.Value);
            if (employeeUser != null)
            {
                await _notificationService.CreateNotificationAsync(
                    employeeUser.UserId,
                    "New Performance Evaluation Assigned",
                    "PerformanceEvaluation",
                    created.EvalId);
            }

            // Notify Evaluators (if any)
            if (request.Responses != null)
            {
                var uniqueEvaluators = request.Responses
                    .Where(r => r.RespondentRole != "Self" && r.RespondentEmployeeId.HasValue)
                    .Select(r => r.RespondentEmployeeId!.Value)
                    .Distinct();

                foreach (var evaluatorId in uniqueEvaluators)
                {
                    var evaluatorUser = await _userRepository.GetByEmployeeIdAsync(evaluatorId);
                    if (evaluatorUser != null)
                    {
                        await _notificationService.CreateNotificationAsync(
                            evaluatorUser.UserId,
                            $"Requested to provide feedback for {created.Employee?.FullName ?? "an employee"}",
                            "360Feedback",
                            created.EvalId);
                    }
                }
            }
        }

        return _mapper.Map<PerformanceEvaluationDto>(created);
    }

    public async Task<PerformanceEvaluationDto?> UpdateAsync(int evalId, UpdatePerformanceEvaluationRequest request)
    {
        var entity = _mapper.Map<PerformanceEvaluation>(request);
        entity.EvalId = evalId;

        var updated = await _repository.UpdateAsync(entity);

        if (request.Responses != null && request.Responses.Any())
        {
            foreach (var respReq in request.Responses)
            {
                if (respReq.ResponseId > 0)
                {
                    var existing = await _responseRepository.GetByIdAsync(respReq.ResponseId);
                    if (existing != null)
                    {
                        existing.RatingValue = respReq.RatingValue;
                        existing.AnswerText = respReq.AnswerText;
                        await _responseRepository.UpdateAsync(existing);
                    }
                }
                else
                {
                    // Add new response during update
                    var newResp = _mapper.Map<AppraisalResponse>(respReq);
                    newResp.EvalId = evalId;
                    await _responseRepository.CreateAsync(newResp);
                }
            }
            
            // Re-calculate totals and FinalRatingScore based on updated responses
            var allResponses = await _responseRepository.GetByEvalIdAsync(evalId);
            var respList = allResponses.ToList();
            if (respList.Any())
            {
                var selfRatings = respList.Where(r => r.RespondentRole == "Self" && r.RatingValue.HasValue).ToList();
                var otherRatings = respList.Where(r => r.RespondentRole != "Self" && r.RatingValue.HasValue).ToList();

                var selfTotal = selfRatings.Sum(r => r.RatingValue ?? 0);
                var managerTotal = respList.Where(r => r.RespondentRole == "Manager").Sum(r => r.RatingValue ?? 0);
                
                // Final score calculation (using other's ratings if available, otherwise self)
                var activeRatings = otherRatings.Any() ? otherRatings : selfRatings;
                var totalPoints = activeRatings.Sum(r => r.RatingValue ?? 0);
                var questionsCount = activeRatings.Count();
                
                if (updated != null)
                {
                    updated.SelfRating = selfTotal;
                    updated.ManagerRating = managerTotal;
                    
                    if (questionsCount > 0)
                    {
                        updated.FinalRatingScore = (decimal)totalPoints / (questionsCount * 5) * 100;
                    }
                    
                    await _repository.UpdateAsync(updated);
                }
            }
        }

        // Notify if Finalized and Generate Auto-Outcome
        if (updated != null && updated.Status == PerformanceEvaluationStatus.Finalized && updated.EmployeeId.HasValue)
        {
            var employeeUser = await _userRepository.GetByEmployeeIdAsync(updated.EmployeeId.Value);
            if (employeeUser != null)
            {
                await _notificationService.CreateNotificationAsync(
                    employeeUser.UserId,
                    "Your Performance Appraisal has been finalized",
                    "PerformanceEvaluation",
                    evalId);
            }

            // Auto-Outcome Logic
            var existingOutcomes = await _outcomeRepository.GetByEmployeeIdAsync(updated.EmployeeId.Value);
            if (!existingOutcomes.Any(o => o.EvalId == evalId))
            {
                var employee = await _employeeRepository.GetByIdAsync(updated.EmployeeId.Value);
                var score = updated.FinalRatingScore ?? 0;
                
                string recommendation = score switch
                {
                    >= 90 => "Promotion",
                    >= 75 => "Salary Increment",
                    >= 50 => "No Change",
                    _ => "PIP"
                };

                var outcome = new PerformanceOutcome
                {
                    EvalId = evalId,
                    EmployeeId = updated.EmployeeId,
                    CycleId = updated.CycleId,
                    RecommendationType = recommendation,
                    OldPositionId = employee?.PositionId,
                    ApprovalStatus = "Pending",
                    EffectiveDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)) // Default to next month
                };

                await _outcomeRepository.CreateAsync(outcome);
            }
        }

        return _mapper.Map<PerformanceEvaluationDto?>(updated);
    }

    public async Task<bool> SubmitSelfAssessmentAsync(int evalId)
    {
        var evaluation = await _repository.GetByIdAsync(evalId);
        if (evaluation == null) return false;

        evaluation.Status = PerformanceEvaluationStatus.SelfSubmitted;
        await _repository.UpdateAsync(evaluation);

        // Notify Manager
        if (evaluation.EmployeeId.HasValue)
        {
            var employee = await _employeeRepository.GetByIdAsync(evaluation.EmployeeId.Value);
            if (employee?.ReportsTo != null)
            {
                var managerUser = await _userRepository.GetByEmployeeIdAsync(employee.ReportsTo.Value);
                if (managerUser != null)
                {
                    await _notificationService.CreateNotificationAsync(
                        managerUser.UserId,
                        $"{employee.FullName} has submitted their self-assessment",
                        "PerformanceEvaluation",
                        evalId);
                }
            }
        }

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
        var responsesList = responses.ToList();

        // Calculate totals for report
        var selfRatings = responsesList.Where(r => r.RespondentRole == "Self" && r.RatingValue.HasValue).ToList();
        var otherRatings = responsesList.Where(r => r.RespondentRole != "Self" && r.RatingValue.HasValue).ToList();
        
        var selfTotal = selfRatings.Sum(r => r.RatingValue ?? 0);
        var managerTotal = responsesList.Where(r => r.RespondentRole == "Manager").Sum(r => r.RatingValue ?? 0);
        
        var activeRatingsForScore = otherRatings.Any() ? otherRatings : selfRatings;
        var totalPoints = activeRatingsForScore.Sum(r => r.RatingValue ?? 0);
        var questionsCount = activeRatingsForScore.Count();
        
        decimal? calculatedScore = questionsCount > 0 ? (decimal)totalPoints / (questionsCount * 5) * 100 : 0;
        var scoreToUse = eval.FinalRatingScore ?? calculatedScore;
        
        var scales = await _ratingScaleRepository.GetAllAsync();
        var band = scales.OrderByDescending(s => s.RatingLevel)
            .FirstOrDefault(s => scoreToUse >= s.RatingLevel);

        return new AppraisalReportDto
        {
            EmployeeName = employee?.FullName ?? "N/A",
            EmployeeCode = employee?.EmployeeCode ?? "N/A",
            DepartmentName = dept?.DepartmentName ?? "N/A",
            PositionTitle = employee?.Position?.PositionTitle ?? "N/A",
            CycleName = cycle?.CycleName ?? "N/A",
            ManagerName = employee?.ReportsToNavigation?.FullName ?? "N/A",
            AssessmentDate = eval.FinalizedAt ?? DateTime.Now,
            EffectiveDate = cycle != null ? new DateTime(cycle.EndDate.Year, cycle.EndDate.Month, cycle.EndDate.Day) : (DateTime?)null,
            FinalScore = scoreToUse,
            PerformanceBand = band?.Label ?? "N/A",
            TotalPoints = totalPoints,
            AnsweredQuestionsCount = questionsCount,
            MaxPoints = questionsCount * 5,
            Responses = _mapper.Map<List<AppraisalResponseDto>>(responsesList)
        };
    }
}
