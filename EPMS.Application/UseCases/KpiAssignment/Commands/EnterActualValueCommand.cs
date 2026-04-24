using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Commands;

public record EnterActualValueCommand(int AssignmentId, decimal ActualValue, int? UserId) : IRequest<Result<KpiScoreSummaryDto>>;

public class EnterActualValueCommandHandler : IRequestHandler<EnterActualValueCommand, Result<KpiScoreSummaryDto>>
{
    private readonly IKpiAssignmentRepository _repository;
    private readonly IKpiScoreCalculator _scoreCalculator;
    private readonly IKpiQueryService _queryService;

    public EnterActualValueCommandHandler(
        IKpiAssignmentRepository repository,
        IKpiScoreCalculator scoreCalculator,
        IKpiQueryService queryService)
    {
        _repository = repository;
        _scoreCalculator = scoreCalculator;
        _queryService = queryService;
    }

    public async Task<Result<KpiScoreSummaryDto>> Handle(EnterActualValueCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetByIdAsync(request.AssignmentId);
        if (assignment == null) return Result<KpiScoreSummaryDto>.Failure("Assignment not found.");

        assignment.SetActualValue(request.ActualValue);

        var score = _scoreCalculator.CalculateKpiScore(request.ActualValue, assignment.TargetValue, assignment.Direction);
        var weightedScore = _scoreCalculator.CalculateWeightedScore(score, assignment.WeightPercent);

        assignment.UpdateScores(score, weightedScore);
        await _repository.UpdateAsync(assignment);

        var summary = await _queryService.GetKpiScoreSummaryAsync(assignment.EmployeeId, assignment.CycleId);
        return Result<KpiScoreSummaryDto>.Success(summary!);
    }
}
