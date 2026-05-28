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
    private readonly IAuditLogService _auditLogService;
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
        IAuditLogService auditLogService,
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
        _auditLogService = auditLogService;
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

            await RecalculateScoresAsync(created.EvalId);
        }

        // Send notifications
        await SendCreationNotificationsAsync(created, request.Responses);

        return _mapper.Map<PerformanceEvaluationDto>(created);
    }

    public async Task<PerformanceEvaluationDto?> UpdateAsync(int evalId, UpdatePerformanceEvaluationRequest request, int? currentEmployeeId = null)
    {
        var existingEval = await _repository.GetByIdAsync(evalId);
        if (existingEval == null) return null;

        _mapper.Map(request, existingEval);
        existingEval.EvalId = evalId;

        if (request.Responses != null && request.Responses.Any())
        {
            foreach (var respReq in request.Responses)
            {
                if (respReq.ResponseId > 0)
                {
                    var existingResp = await _responseRepository.GetByIdAsync(respReq.ResponseId);
                    if (existingResp != null)
                    {
                        existingResp.RatingValue = respReq.RatingValue;
                        existingResp.AnswerText = respReq.AnswerText;
                        await _responseRepository.UpdateAsync(existingResp);
                    }
                }
                else
                {
                    var newResp = _mapper.Map<AppraisalResponse>(respReq);
                    newResp.EvalId = evalId;
                    await _responseRepository.CreateAsync(newResp);
                }
            }
        }

        await RecalculateScoresAsync(evalId);
        var updated = await _repository.GetByIdAsync(evalId);

        // Notify if Finalized and Generate Auto-Outcome
        if (updated != null && updated.Status == PerformanceEvaluationStatus.Finalized)
        {
            await HandleFinalizationAsync(updated);
        }

        if (updated != null && updated.Status == PerformanceEvaluationStatus.Finalized)
        {
            await _auditLogService.LogAsync("PerformanceEvaluation", "Finalize", evalId, $"Evaluation finalized with score {updated.FinalRatingScore}", currentEmployeeId);
        }

        return _mapper.Map<PerformanceEvaluationDto?>(updated);
    }

    private async Task RecalculateScoresAsync(int evalId)
    {
        var evaluation = await _repository.GetByIdAsync(evalId);
        if (evaluation == null) return;

        var responses = (await _responseRepository.GetByEvalIdAsync(evalId)).ToList();
        if (!responses.Any()) return;

        var form = evaluation.Form;
        var formType = (AppraisalFormType)evaluation.FormId; // Assuming FormId matches enum for simplicity, or fetch from evaluation.Form.FormType

        // 1. Calculate Totals by Role
        var selfRatings = responses.Where(r => r.RespondentRole == "Self" && r.RatingValue.HasValue).ToList();
        var managerRatings = responses.Where(r => r.RespondentRole == "Manager" && r.RatingValue.HasValue).ToList();
        var peerRatings = responses.Where(r => r.RespondentRole == "Peer" && r.RatingValue.HasValue).ToList();
        var subRatings = responses.Where(r => r.RespondentRole == "Subordinate" && r.RatingValue.HasValue).ToList();
        var externalRatings = responses.Where(r => r.RespondentRole == "External" && r.RatingValue.HasValue).ToList();

        evaluation.SelfRating = selfRatings.Sum(r => r.RatingValue ?? 0);
        evaluation.ManagerRating = managerRatings.Sum(r => r.RatingValue ?? 0);

        // 2. Calculate Final Weighted Score based on Form Type
        decimal finalScore = 0;

        switch (evaluation.Form?.FormType ?? AppraisalFormType.PerformanceAppraisal)
        {
            case AppraisalFormType.SelfAssessment:
                // 100% Self
                finalScore = CalculateAverageScore(selfRatings);
                break;

            case AppraisalFormType.PerformanceAppraisal:
                // 80% Manager, 20% Self
                decimal mgrAvg = CalculateAverageScore(managerRatings);
                decimal selfAvg = CalculateAverageScore(selfRatings);
                
                if (managerRatings.Any() && selfRatings.Any())
                    finalScore = (mgrAvg * 0.8m) + (selfAvg * 0.2m);
                else if (managerRatings.Any())
                    finalScore = mgrAvg;
                else
                    finalScore = selfAvg;
                break;

            case AppraisalFormType.Feedback360:
                // Weighted average of all sources
                // Peers: 40%, Subordinates: 30%, Manager: 20%, External: 10%
                decimal pAvg = CalculateAverageScore(peerRatings);
                decimal sAvg = CalculateAverageScore(subRatings);
                decimal mAvg = CalculateAverageScore(managerRatings);
                decimal eAvg = CalculateAverageScore(externalRatings);

                int sources = 0;
                if (peerRatings.Any()) { finalScore += pAvg * 0.4m; sources++; }
                if (subRatings.Any()) { finalScore += sAvg * 0.3m; sources++; }
                if (managerRatings.Any()) { finalScore += mAvg * 0.2m; sources++; }
                if (externalRatings.Any()) { finalScore += eAvg * 0.1m; sources++; }

                // If some sources are missing, re-normalize or just use available
                if (sources > 0)
                {
                    // For 360, we might just want a straight average if weights don't add to 100% due to missing sources
                    // but standard 360 often just averages all 'other' ratings equally
                    var allOtherRatings = responses.Where(r => r.RespondentRole != "Self" && r.RatingValue.HasValue).ToList();
                    finalScore = CalculateAverageScore(allOtherRatings);
                }
                break;
        }

        evaluation.FinalRatingScore = finalScore;
        await _repository.UpdateAsync(evaluation);
    }

    private decimal CalculateAverageScore(List<AppraisalResponse> ratings)
    {
        if (!ratings.Any()) return 0;
        return (decimal)ratings.Average(r => r.RatingValue ?? 0) / 5 * 100;
    }

    private async Task SendCreationNotificationsAsync(PerformanceEvaluation created, List<CreateAppraisalResponseRequest>? responses)
    {
        if (!created.EmployeeId.HasValue) return;

        var employeeUser = await _userRepository.GetByEmployeeIdAsync(created.EmployeeId.Value);
        if (employeeUser != null)
        {
            await _notificationService.CreateNotificationAsync(
                employeeUser.UserId,
                $"New {created.Form?.FormName ?? "Performance Evaluation"} Assigned",
                "PerformanceEvaluation",
                created.EvalId);
        }

        if (responses != null)
        {
            var uniqueEvaluators = responses
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

    private async Task HandleFinalizationAsync(PerformanceEvaluation updated)
    {
        if (!updated.EmployeeId.HasValue) return;

        var employeeUser = await _userRepository.GetByEmployeeIdAsync(updated.EmployeeId.Value);
        if (employeeUser != null)
        {
            await _notificationService.CreateNotificationAsync(
                employeeUser.UserId,
                "Your Performance Appraisal has been finalized",
                "PerformanceEvaluation",
                updated.EvalId);
        }

        // Auto-Outcome Logic
        var existingOutcomes = await _outcomeRepository.GetByEmployeeIdAsync(updated.EmployeeId.Value);
        if (!existingOutcomes.Any(o => o.EvalId == updated.EvalId))
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
                EvalId = updated.EvalId,
                EmployeeId = updated.EmployeeId,
                CycleId = updated.CycleId,
                RecommendationType = recommendation,
                OldPositionId = employee?.PositionId,
                ApprovalStatus = recommendation == "PIP" ? "Requires HR Review" : "Pending",
                EffectiveDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1))
            };

            await _outcomeRepository.CreateAsync(outcome);
            
            // Special notification for PIP
            if (recommendation == "PIP")
            {
                // Notify HR/Admins (this would typically be a specific group notification)
                // For now, we'll assume there's a mechanism to notify those with PerformanceOutcomes.Manage permission
            }
        }
    }

    public async Task<bool> SubmitSelfAssessmentAsync(int evalId, int? currentEmployeeId = null)
    {
        var evaluation = await _repository.GetByIdAsync(evalId);
        if (evaluation == null) return false;

        evaluation.Status = PerformanceEvaluationStatus.SelfSubmitted;
        await _repository.UpdateAsync(evaluation);

        await _auditLogService.LogAsync("PerformanceEvaluation", "SubmitSelf", evalId, "Employee submitted self-assessment", currentEmployeeId);

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
                        $"{employee.FullName} has submitted their self-assessment. You can now start the appraisal.",
                        "PerformanceEvaluation",
                        evalId);
                }
            }
        }

        return true;
    }

    public async Task<bool> SubmitManagerReviewAsync(int evalId, int? currentEmployeeId = null)
    {
        var evaluation = await _repository.GetByIdAsync(evalId);
        if (evaluation == null) return false;

        // If it's a Performance Appraisal, it goes to Calibration before Finalization
        if (evaluation.Form?.FormType == AppraisalFormType.PerformanceAppraisal)
        {
            evaluation.Status = PerformanceEvaluationStatus.AwaitingCalibration;
        }
        else
        {
            evaluation.Status = PerformanceEvaluationStatus.ManagerReviewed;
        }

        await _repository.UpdateAsync(evaluation);

        await _auditLogService.LogAsync("PerformanceEvaluation", "SubmitMgr", evalId, $"Manager submitted review. Status: {evaluation.Status}", currentEmployeeId);

        // Notify Senior Managers (L04+) if awaiting calibration
        if (evaluation.Status == PerformanceEvaluationStatus.AwaitingCalibration)
        {
            var seniorManagers = await _employeeRepository.GetAllAsync();
            var targetManagers = seniorManagers.Where(e => ParseLevel(e.Position?.LevelId) <= 4);
            
            foreach (var mgr in targetManagers)
            {
                var mgrUser = await _userRepository.GetByEmployeeIdAsync(mgr.EmployeeId);
                if (mgrUser != null)
                {
                    await _notificationService.CreateNotificationAsync(
                        mgrUser.UserId,
                        $"New Performance Calibration required for {evaluation.Employee?.FullName ?? "an employee"}",
                        "PerformanceCalibration",
                        evalId);
                }
            }
        }

        // Notify Employee
        if (evaluation.EmployeeId.HasValue)
        {
            var employeeUser = await _userRepository.GetByEmployeeIdAsync(evaluation.EmployeeId.Value);
            if (employeeUser != null)
            {
                await _notificationService.CreateNotificationAsync(
                    employeeUser.UserId,
                    "Your manager has completed your performance appraisal review.",
                    "PerformanceEvaluation",
                    evalId);
            }
        }

        return true;
    }

    public async Task<bool> DeleteAsync(int evalId)
    {
        return await _repository.DeleteAsync(evalId);
    }

    public async Task<bool> ReopenAsync(int evalId, int? currentEmployeeId = null)
    {
        var evaluation = await _repository.GetByIdAsync(evalId);
        if (evaluation == null) return false;

        evaluation.Status = PerformanceEvaluationStatus.Draft;
        evaluation.IsFinalized = false;
        evaluation.FinalizedAt = null;
        
        await _repository.UpdateAsync(evaluation);

        // Log the re-open action for audit purposes
        await _auditLogService.LogAsync("PerformanceEvaluation", "Reopen", evalId, "Review re-opened by Admin", currentEmployeeId);

        // Notify Employee that their evaluation is re-opened
        if (evaluation.EmployeeId.HasValue)
        {
            var employeeUser = await _userRepository.GetByEmployeeIdAsync(evaluation.EmployeeId.Value);
            if (employeeUser != null)
            {
                await _notificationService.CreateNotificationAsync(
                    employeeUser.UserId,
                    "Your Performance Evaluation has been re-opened for adjustments.",
                    "PerformanceEvaluation",
                    evalId);
            }
        }

        return true;
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

    private int ParseLevel(string? levelId)
    {
        if (string.IsNullOrEmpty(levelId)) return 99;
        var numericPart = new string(levelId.Where(char.IsDigit).ToArray());
        return int.TryParse(numericPart, out int level) ? level : 99;
    }
}
