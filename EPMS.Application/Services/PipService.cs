using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.DTOs;
using EPMS.Shared.Requests;
using EPMS.Shared.Responses;
using EPMS.Domain.SpResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EPMS.Application.Services
{
    public sealed class PipService : IPipService
    {
        private readonly IPipPlanRepository _planRepo;
        private readonly IPipObjectiveRepository _objectiveRepo;
        private readonly IPipMeetingRepository _meetingRepo;
        private readonly IPipReportRepository _reportRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<PipService> _logger;
        private readonly IValidator<CreatePipPlanRequest> _createPipValidator;
        private readonly IValidator<CreatePipObjectiveRequest> _createObjValidator;
        private readonly IValidator<CreatePipMeetingRequest> _createMeetingValidator;

        public PipService(
            IPipPlanRepository planRepo,
            IPipObjectiveRepository objectiveRepo,
            IPipMeetingRepository meetingRepo,
            IPipReportRepository reportRepo,
            IMapper mapper,
            ILogger<PipService> logger,
            IValidator<CreatePipPlanRequest> createPipValidator,
            IValidator<CreatePipObjectiveRequest> createObjValidator,
            IValidator<CreatePipMeetingRequest> createMeetingValidator)
        {
            _planRepo = planRepo;
            _objectiveRepo = objectiveRepo;
            _meetingRepo = meetingRepo;
            _reportRepo = reportRepo;
            _mapper = mapper;
            _logger = logger;
            _createPipValidator = createPipValidator;
            _createObjValidator = createObjValidator;
            _createMeetingValidator = createMeetingValidator;
        }

        // ── PIP PLANS ────────────────────────────────────────────────────────────

        public async Task<ApiResponse<PipPlanDto>> GetPipByIdAsync(int pipId, CancellationToken ct = default)
        {
            var plan = await _planRepo.GetByIdWithDetailsAsync(pipId, ct);
            if (plan is null)
                return ApiResponse<PipPlanDto>.Fail($"PIP with ID {pipId} not found.");

            return ApiResponse<PipPlanDto>.Ok(_mapper.Map<PipPlanDto>(plan));
        }

        public async Task<ApiResponse<IEnumerable<PipPlanDto>>> GetAllPipsAsync(CancellationToken ct = default)
        {
            var plans = await _planRepo.GetAllAsync(ct);
            return ApiResponse<IEnumerable<PipPlanDto>>.Ok(_mapper.Map<IEnumerable<PipPlanDto>>(plans));
        }

        public async Task<ApiResponse<IEnumerable<PipPlanDto>>> GetPipsByEmployeeAsync(
            int employeeId, CancellationToken ct = default)
        {
            var plans = await _planRepo.GetByEmployeeIdAsync(employeeId, ct);
            return ApiResponse<IEnumerable<PipPlanDto>>.Ok(_mapper.Map<IEnumerable<PipPlanDto>>(plans));
        }

        public async Task<ApiResponse<IEnumerable<PipPlanDto>>> GetPipsByManagerAsync(
            int managerId, CancellationToken ct = default)
        {
            var plans = await _planRepo.GetByManagerIdAsync(managerId, ct);
            return ApiResponse<IEnumerable<PipPlanDto>>.Ok(_mapper.Map<IEnumerable<PipPlanDto>>(plans));
        }

        public async Task<ApiResponse<PipPlanDto>> CreatePipAsync(
            CreatePipPlanRequest request, CancellationToken ct = default)
        {
            // Server-side validation (FluentValidation)
            var validation = await _createPipValidator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResponse<PipPlanDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            // Business rule: Employee cannot have two active PIPs
            if (await _planRepo.HasActivePipAsync(request.EmployeeId, ct))
                return ApiResponse<PipPlanDto>.Fail(
                    "This employee already has an active PIP. Close or complete it before creating a new one.");

            var plan = _mapper.Map<PipPlan>(request);

            // Map nested objectives
            foreach (var objRequest in request.Objectives)
            {
                plan.PipObjectives.Add(_mapper.Map<PipObjective>(objRequest));
            }

            var created = await _planRepo.CreateAsync(plan, ct);
            _logger.LogInformation("PIP Plan {PipId} created for Employee {EmployeeId}.",
                created.Pipid, created.EmployeeId);

            var dto = _mapper.Map<PipPlanDto>(
                await _planRepo.GetByIdWithDetailsAsync(created.Pipid, ct));

            return ApiResponse<PipPlanDto>.Ok(dto, "PIP created successfully.");
        }

        public async Task<ApiResponse<PipPlanDto>> UpdatePipAsync(
            UpdatePipPlanRequest request, CancellationToken ct = default)
        {
            var plan = await _planRepo.GetByIdAsync(request.PipId, ct);
            if (plan is null)
                return ApiResponse<PipPlanDto>.Fail($"PIP with ID {request.PipId} not found.");

            plan.EndDate = request.EndDate;
            plan.Status = request.Status;
            plan.OverallGoal = request.OverallGoal;

            await _planRepo.UpdateAsync(plan, ct);
            var updated = await _planRepo.GetByIdWithDetailsAsync(plan.Pipid, ct);
            return ApiResponse<PipPlanDto>.Ok(_mapper.Map<PipPlanDto>(updated), "PIP updated successfully.");
        }

        public async Task<ApiResponse<bool>> DeletePipAsync(int pipId, CancellationToken ct = default)
        {
            var deleted = await _planRepo.DeleteAsync(pipId, ct);
            return deleted
                ? ApiResponse<bool>.Ok(true, "PIP deleted.")
                : ApiResponse<bool>.Fail("PIP not found.");
        }

        // ── OBJECTIVES ───────────────────────────────────────────────────────────

        public async Task<ApiResponse<PipObjectiveDto>> AddObjectiveAsync(
            CreatePipObjectiveRequest request, CancellationToken ct = default)
        {
            var validation = await _createObjValidator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResponse<PipObjectiveDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var plan = await _planRepo.GetByIdAsync(request.PipId, ct);
            if (plan is null)
                return ApiResponse<PipObjectiveDto>.Fail($"PIP with ID {request.PipId} not found.");

            var objective = _mapper.Map<PipObjective>(request);
            var created = await _objectiveRepo.CreateAsync(objective, ct);
            return ApiResponse<PipObjectiveDto>.Ok(_mapper.Map<PipObjectiveDto>(created), "Objective added.");
        }

        public async Task<ApiResponse<PipObjectiveDto>> UpdateObjectiveAsync(
            UpdatePipObjectiveRequest request, CancellationToken ct = default)
        {
            var objective = await _objectiveRepo.GetByIdAsync(request.ObjectiveId, ct);
            if (objective is null)
                return ApiResponse<PipObjectiveDto>.Fail($"Objective {request.ObjectiveId} not found.");

            objective.IsAchieved = request.IsAchieved;
            objective.ReviewComments = request.ReviewComments;
            objective.SuccessCriteria = request.SuccessCriteria;

            var updated = await _objectiveRepo.UpdateAsync(objective, ct);
            return ApiResponse<PipObjectiveDto>.Ok(_mapper.Map<PipObjectiveDto>(updated), "Objective updated.");
        }

        public async Task<ApiResponse<bool>> DeleteObjectiveAsync(int objectiveId, CancellationToken ct = default)
        {
            var deleted = await _objectiveRepo.DeleteAsync(objectiveId, ct);
            return deleted
                ? ApiResponse<bool>.Ok(true, "Objective deleted.")
                : ApiResponse<bool>.Fail("Objective not found.");
        }

        // ── MEETINGS ─────────────────────────────────────────────────────────────

        public async Task<ApiResponse<PipMeetingDto>> AddMeetingAsync(
            CreatePipMeetingRequest request, CancellationToken ct = default)
        {
            var validation = await _createMeetingValidator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return ApiResponse<PipMeetingDto>.Fail("Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage).ToList());

            var meeting = _mapper.Map<PipMeeting>(request);
            var created = await _meetingRepo.CreateAsync(meeting, ct);
            return ApiResponse<PipMeetingDto>.Ok(_mapper.Map<PipMeetingDto>(created), "Meeting scheduled.");
        }

        public async Task<ApiResponse<PipMeetingDto>> UpdateMeetingAsync(
            UpdatePipMeetingRequest request, CancellationToken ct = default)
        {
            var meeting = await _meetingRepo.GetByIdAsync(request.PipMeetingId, ct);
            if (meeting is null)
                return ApiResponse<PipMeetingDto>.Fail($"Meeting {request.PipMeetingId} not found.");

            meeting.MeetingDate = request.MeetingDate;
            meeting.DiscussionPoints = request.DiscussionPoints;
            meeting.ProgressStatus = request.ProgressStatus;
            meeting.NextSteps = request.NextSteps;

            var updated = await _meetingRepo.UpdateAsync(meeting, ct);
            return ApiResponse<PipMeetingDto>.Ok(_mapper.Map<PipMeetingDto>(updated), "Meeting updated.");
        }

        public async Task<ApiResponse<bool>> DeleteMeetingAsync(int meetingId, CancellationToken ct = default)
        {
            var deleted = await _meetingRepo.DeleteAsync(meetingId, ct);
            return deleted
                ? ApiResponse<bool>.Ok(true, "Meeting deleted.")
                : ApiResponse<bool>.Fail("Meeting not found.");
        }

        // ── REPORTS (Dapper) ─────────────────────────────────────────────────────

        public async Task<ApiResponse<IEnumerable<PipReportResult>>> GetSummaryReportAsync(
            int? managerId = null, string? status = null, CancellationToken ct = default)
        {
            var results = await _reportRepo.GetPipSummaryReportAsync(managerId, status, ct);
            return ApiResponse<IEnumerable<PipReportResult>>.Ok(results);
        }
    }
}
