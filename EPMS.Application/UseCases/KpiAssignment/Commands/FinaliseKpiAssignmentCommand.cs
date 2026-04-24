using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Domain.Services;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Commands;

public record FinaliseKpiAssignmentCommand(int EmployeeId, int CycleId, int? UserId) : IRequest<Result>;

public class FinaliseKpiAssignmentCommandHandler : IRequestHandler<FinaliseKpiAssignmentCommand, Result>
{
    private readonly IKpiAssignmentRepository _repository;
    private readonly KpiWeightValidator _weightValidator;
    private readonly IAuditLogService _auditLogService;

    public FinaliseKpiAssignmentCommandHandler(
        IKpiAssignmentRepository repository,
        KpiWeightValidator weightValidator,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _weightValidator = weightValidator;
        _auditLogService = auditLogService;
    }

    public async Task<Result> Handle(FinaliseKpiAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignments = await _repository.GetAssignmentsByEmployeeCycleAsync(request.EmployeeId, request.CycleId);

        var weightResult = _weightValidator.ValidateTotalWeight(assignments.Select(a => a.WeightPercent));
        if (!weightResult.IsValid) return Result.Failure(weightResult.ErrorMessage);

        foreach (var assignment in assignments)
        {
            assignment.Activate();
            await _repository.UpdateAsync(assignment);
        }

        await _auditLogService.LogAsync("EmployeeKpiAssignment", "Finalise", null, $"Finalised assignments for employee {request.EmployeeId} cycle {request.CycleId}", request.UserId);

        return Result.Success();
    }
}
