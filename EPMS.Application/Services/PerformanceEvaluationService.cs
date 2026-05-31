using System.Diagnostics;
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
    private readonly IAppraisalFormRepository _formRepository;
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
        IAppraisalFormRepository formRepository,
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
        _formRepository = formRepository;
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

    public async Task<PerformanceEvaluationDto> CreateAsync(CreatePerformanceEvaluationRequest request, int? currentEmployeeId = null)
    {
        // Server-side Security Validation
        if (currentEmployeeId.HasValue)
        {
            var creator = await _employeeRepository.GetByIdAsync(currentEmployeeId.Value);
            var subject = request.EmployeeId.HasValue ? await _employeeRepository.GetByIdAsync(request.EmployeeId.Value) : null;
            var form = await _formRepository.GetByIdAsync(request.FormId);
            
            if (creator != null && subject != null && form != null)
            {
                var creatorLevel = ParseLevel(creator.Position?.LevelId);
                var subjectLevel = ParseLevel(subject.Position?.LevelId);
                
                // 1. Self-Assessment: Only for self (L02-L09)
                if (form.FormType == AppraisalFormType.SelfAssessment)
                {
                    if (creator.EmployeeId != subject.EmployeeId)
                        throw new UnauthorizedAccessException("Self-assessment can only be initiated for yourself.");
                    if (creatorLevel < 2 || creatorLevel > 9)
                        throw new UnauthorizedAccessException("You are not eligible for self-assessment based on your level.");
                }
                
                // 2. Performance Appraisal: Only for direct reports (L02-L06)
                else if (form.FormType == AppraisalFormType.PerformanceAppraisal)
                {
                    if (creatorLevel < 2 || creatorLevel > 6)
                        throw new UnauthorizedAccessException("Only levels 02-06 can initiate performance appraisals.");
                    if (subject.ReportsTo != creator.EmployeeId && creatorLevel > 4) // Senior management (L01-L04) can oversee
                        throw new UnauthorizedAccessException("You can only initiate appraisals for your direct reports.");
                }
                
                // 3. 360-Feedback: L02-L08
                else if (form.FormType == AppraisalFormType.Feedback360)
                {
                    if (creatorLevel < 2 || creatorLevel > 8)
                        throw new UnauthorizedAccessException("Only levels 02-08 can initiate 360-feedback.");
                }
                
                // 4. Calibration: L01-L04
                else if (form.FormType == AppraisalFormType.CalibrationReview)
                {
                    if (creatorLevel < 1 || creatorLevel > 4)
                        throw new UnauthorizedAccessException("Only Senior Management (L01-L04) can initiate calibration reviews.");
                }
            }
        }

        var entity = _mapper.Map<PerformanceEvaluation>(request);
        entity.CreatedByEmployeeId = currentEmployeeId;
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
        await SendCreationNotificationsAsync(created, request.Responses, currentEmployeeId);

        return _mapper.Map<PerformanceEvaluationDto>(created);
    }

    public async Task<int> CreateBulkAsync(BulkPerformanceEvaluationRequest request, int? currentEmployeeId = null)
    {
        int count = 0;
        var form = await _formRepository.GetByIdAsync(request.FormId);
        if (form == null) return 0;

        foreach (var employeeId in request.EmployeeIds)
        {
            try
            {
                var createRequest = new CreatePerformanceEvaluationRequest
                {
                    EmployeeId = employeeId,
                    CycleId = request.CycleId,
                    FormId = request.FormId,
                    Status = PerformanceEvaluationStatus.Draft,
                    CreatedByEmployeeId = currentEmployeeId,
                    IsFinalized = false
                };

                await CreateAsync(createRequest, currentEmployeeId);
                count++;
            }
            catch (Exception ex)
            {
                // Log error but continue with other employees
                await _auditLogService.LogAsync("PerformanceEvaluation", "BulkCreateError", employeeId, $"Error creating bulk evaluation: {ex.Message}", currentEmployeeId);
            }
        }

        await _auditLogService.LogAsync("PerformanceEvaluation", "BulkCreate", request.CycleId, $"Bulk created {count} evaluations for form {form.FormName}", currentEmployeeId);
        return count;
    }

    public async Task<PerformanceEvaluationDto?> UpdateAsync(int evalId, UpdatePerformanceEvaluationRequest request, int? currentEmployeeId = null)
    {
        var existingEval = await _repository.GetByIdAsync(evalId);
        if (existingEval == null) return null;

        // Server-side Security Validation
        if (currentEmployeeId.HasValue)
        {
            var user = await _employeeRepository.GetByIdAsync(currentEmployeeId.Value);
            if (user != null)
            {
                var userLevel = ParseLevel(user.Position?.LevelId);
                bool isSubject = existingEval.EmployeeId == user.EmployeeId;
                
                // Finalized evaluations cannot be edited
                if (existingEval.Status == PerformanceEvaluationStatus.Finalized)
                    throw new InvalidOperationException("Finalized evaluations cannot be edited.");

                // Subject can only edit their own evaluation if it's in Draft status
                if (isSubject && existingEval.Status != PerformanceEvaluationStatus.Draft)
                    throw new UnauthorizedAccessException("You can only edit your own evaluation while it is in Draft status.");

                // If not the subject, must have appropriate level or be the manager
                if (!isSubject)
                {
                    bool isManager = existingEval.Employee?.ReportsTo == user.EmployeeId;
                    if (!isManager && userLevel > 4) // Only managers or Senior Mgmt (L01-L04)
                        throw new UnauthorizedAccessException("You do not have permission to edit this evaluation.");
                }
            }
        }

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
        var sw = Stopwatch.StartNew();
        var evaluation = await _repository.GetByIdAsync(evalId);
        if (evaluation == null) return;

        var responses = (await _responseRepository.GetByEvalIdAsync(evalId)).ToList();
        if (!responses.Any()) return;

        var form = evaluation.Form;

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

            case AppraisalFormType.CalibrationReview:
                // 100% Manager/Senior Management review
                finalScore = CalculateAverageScore(managerRatings);
                break;
        }

        evaluation.FinalRatingScore = finalScore;
        await _repository.UpdateAsync(evaluation);

        sw.Stop();
        await _auditLogService.LogAsync("PerformanceEvaluation", "RecalculateScores", evalId, $"Recalculated scores in {sw.ElapsedMilliseconds}ms. Final Score: {finalScore}", null);
    }

    private decimal CalculateAverageScore(List<AppraisalResponse> ratings)
    {
        if (!ratings.Any()) return 0;
        return (decimal)ratings.Average(r => r.RatingValue ?? 0) / 5 * 100;
    }

    private async Task SendCreationNotificationsAsync(PerformanceEvaluation created, List<CreateAppraisalResponseRequest>? responses, int? creatorEmployeeId)
    {
        if (!created.EmployeeId.HasValue) return;

        var subject = await _employeeRepository.GetByIdAsync(created.EmployeeId.Value);
        var form = await _formRepository.GetByIdAsync(created.FormId);
        
        if (subject == null || form == null) return;

        // 1. Notify Subject (Employee)
        var subjectUser = await _userRepository.GetByEmployeeIdAsync(subject.EmployeeId);
        if (subjectUser != null && subjectUser.EmployeeId != creatorEmployeeId)
        {
            var creator = creatorEmployeeId.HasValue ? await _employeeRepository.GetByIdAsync(creatorEmployeeId.Value) : null;
            string creatorName = creator?.FullName ?? "your manager";

            string subjectMessage = form.FormType switch
            {
                AppraisalFormType.SelfAssessment => $"Action Required: Please submit your {form.FormName} to {creatorName}.",
                AppraisalFormType.PerformanceAppraisal => $"Action Required: Your {form.FormName} cycle has started. Please complete your self-appraisal section.",
                AppraisalFormType.Feedback360 => $"Notification: A {form.FormName} has been initiated for you. You will receive feedback from your peers soon.",
                _ => $"A new {form.FormName} has been initiated for you by {creatorName}."
            };

            await _notificationService.CreateNotificationAsync(
                subjectUser.UserId,
                subjectMessage,
                "PerformanceEvaluation",
                created.EvalId);
        }

        // 2. Notify Evaluators (Managers, Peers, etc.)
        if (responses != null)
        {
            var uniqueEvaluators = responses
                .Where(r => r.RespondentRole != "Self" && r.RespondentEmployeeId.HasValue)
                .Select(r => r.RespondentEmployeeId!.Value)
                .Distinct();

            foreach (var evaluatorId in uniqueEvaluators)
            {
                // Skip notifying the person who created the form
                if (evaluatorId == creatorEmployeeId) continue;

                var evaluatorUser = await _userRepository.GetByEmployeeIdAsync(evaluatorId);
                if (evaluatorUser != null)
                {
                    string evaluatorMessage = $"Action Required: Please provide {form.FormName} feedback for {subject.FullName}.";
                    
                    await _notificationService.CreateNotificationAsync(
                        evaluatorUser.UserId,
                        evaluatorMessage,
                        form.FormType == AppraisalFormType.Feedback360 ? "360Feedback" : "PerformanceEvaluation",
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
                "Your Performance Evaluation has been checked and finalized by the creator.",
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

        // Notify Creator
        if (evaluation.CreatedByEmployeeId.HasValue)
        {
            var creatorUser = await _userRepository.GetByEmployeeIdAsync(evaluation.CreatedByEmployeeId.Value);
            var employee = await _employeeRepository.GetByIdAsync(evaluation.EmployeeId ?? 0);
            
            if (creatorUser != null && employee != null)
            {
                await _notificationService.CreateNotificationAsync(
                    creatorUser.UserId,
                    $"{employee.FullName} has submitted their self-assessment. Please check, lock and finalize the form.",
                    "PerformanceEvaluation",
                    evalId);
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

    public async Task<bool> FinalizeAsync(int evalId, int? currentEmployeeId = null)
    {
        var evaluation = await _repository.GetByIdAsync(evalId);
        if (evaluation == null) return false;

        // Only the creator can finalize
        if (currentEmployeeId.HasValue && evaluation.CreatedByEmployeeId != currentEmployeeId)
        {
            throw new UnauthorizedAccessException("Only the form creator can finalize this evaluation.");
        }

        evaluation.Status = PerformanceEvaluationStatus.Finalized;
        evaluation.IsFinalized = true;
        evaluation.FinalizedAt = DateTime.Now;

        await _repository.UpdateAsync(evaluation);

        await _auditLogService.LogAsync("PerformanceEvaluation", "Finalize", evalId, "Form creator finalized and locked the form", currentEmployeeId);

        await HandleFinalizationAsync(evaluation);

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
        
        // Map to DTOs first so we can access the RespondentName/Position/Department properties
        var responsesDtoList = _mapper.Map<List<AppraisalResponseDto>>(responses.ToList());

        // Fetch evaluator details for the report (Position/Department)
        var employeeList = (await _employeeRepository.GetAllAsync()).ToList();
        foreach (var resp in responsesDtoList)
        {
            if (resp.RespondentId.HasValue)
            {
                var evaluator = employeeList.FirstOrDefault(e => e.EmployeeId == resp.RespondentId.Value);
                if (evaluator != null)
                {
                    resp.RespondentName = evaluator.FullName;
                    resp.RespondentPosition = evaluator.Position?.PositionTitle;
                    resp.RespondentDepartment = evaluator.Department?.DepartmentName;
                }
            }
        }

        // Calculate totals for report
        var selfRatings = responsesDtoList.Where(r => r.RespondentRole == "Self" && r.RatingValue.HasValue).ToList();
        var otherRatings = responsesDtoList.Where(r => r.RespondentRole != "Self" && r.RatingValue.HasValue).ToList();
        
        var selfTotal = selfRatings.Sum(r => r.RatingValue ?? 0);
        var managerTotal = responsesDtoList.Where(r => r.RespondentRole == "Manager").Sum(r => r.RatingValue ?? 0);
        
        var activeRatingsForScore = otherRatings.Any() ? otherRatings : selfRatings;
        var totalPoints = activeRatingsForScore.Sum(r => r.RatingValue ?? 0);
        var questionsCount = activeRatingsForScore.Count();
        
        // Fetch Finalizer details
        Employee? finalizer = null;
        if (eval.CreatedByEmployeeId.HasValue)
        {
            finalizer = await _employeeRepository.GetByIdAsync(eval.CreatedByEmployeeId.Value);
        }
        
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
            SelfComments = eval.SelfComments,
            ManagerComments = eval.ManagerComments,
            CalibrationComments = eval.CalibrationComments,
            EmployeeLevel = employee?.Position?.LevelId,
            SelfRating = eval.SelfRating,
            ManagerRating = eval.ManagerRating,
            TotalPoints = totalPoints,
            AnsweredQuestionsCount = questionsCount,
            MaxPoints = questionsCount * 5,
            FinalizedByName = finalizer?.FullName ?? "N/A",
            FinalizedByDesignation = finalizer?.Position?.PositionTitle ?? "N/A",
            Responses = responsesDtoList
        };
    }

    public async Task<IEnumerable<CalibrationTrendDto>> GetCalibrationTrendAsync()
    {
        var evals = await _repository.GetAllAsync();
        var finalizedEvals = evals.Where(e => e.IsFinalized == true && !string.IsNullOrEmpty(e.CalibrationComments)).ToList();
        
        var employees = await _employeeRepository.GetAllAsync();
        var employeeList = employees.ToList();

        var trends = finalizedEvals
            .GroupBy(e => e.Employee?.ReportsTo)
            .Where(g => g.Key.HasValue)
            .Select(g => 
            {
                var manager = employeeList.FirstOrDefault(e => e.EmployeeId == g.Key.Value);
                var total = g.Count();
                var adjusted = g.Count(e => !string.IsNullOrEmpty(e.CalibrationComments));
                
                return new CalibrationTrendDto
                {
                    ManagerId = g.Key.Value,
                    ManagerName = manager?.FullName ?? "Unknown",
                    TotalEvaluations = total,
                    AdjustedCount = adjusted,
                    AverageAdjustment = total > 0 ? (decimal)adjusted / total * 100 : 0,
                    TrendStatus = (adjusted > total * 0.5) ? "High-Bias" : "Consistent"
                };
            })
            .OrderByDescending(t => t.AverageAdjustment);

        return trends;
    }

    private int ParseLevel(string? levelId)
    {
        if (string.IsNullOrEmpty(levelId)) return 99;
        var numericPart = new string(levelId.Where(char.IsDigit).ToArray());
        return int.TryParse(numericPart, out int level) ? level : 99;
    }
}
