using EPMS.Domain.Enums;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Commands;

public record UpdateKpiAssignmentCommand(
    int AssignmentId,
    string KpiName,
    string? Category,
    string? Unit,
    KpiDirection Direction,
    decimal WeightPercent,
    decimal TargetValue,
    decimal? ActualValue
) : IRequest<Result<bool>>;

public class UpdateKpiAssignmentCommandHandler : IRequestHandler<UpdateKpiAssignmentCommand, Result<bool>>
{
    private readonly EPMS.Domain.Interfaces.IKpiAssignmentRepository _repository;
    private readonly EPMS.Domain.Interfaces.IUnitOfWork _unitOfWork;
    private readonly EPMS.Domain.Interfaces.IKpiScoreCalculator _scoreCalculator;

    public UpdateKpiAssignmentCommandHandler(
        EPMS.Domain.Interfaces.IKpiAssignmentRepository repository,
        EPMS.Domain.Interfaces.IUnitOfWork unitOfWork,
        EPMS.Domain.Interfaces.IKpiScoreCalculator scoreCalculator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _scoreCalculator = scoreCalculator;
    }

    public async Task<Result<bool>> Handle(UpdateKpiAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetByIdAsync(request.AssignmentId);
        if (assignment == null)
            return Result<bool>.Failure("Assignment not found.");

        assignment.UpdateDetails(
            request.KpiName,
            request.Category,
            request.Unit,
            request.Direction,
            request.WeightPercent,
            request.TargetValue);

        if (request.ActualValue.HasValue && assignment.Status == KpiAssignmentStatus.Active)
        {
            assignment.SetActualValue(request.ActualValue.Value);
            var kpiScore = _scoreCalculator.CalculateKpiScore(
                request.ActualValue.Value,
                request.TargetValue,
                request.Direction);
            var weightedScore = _scoreCalculator.CalculateWeightedScore(kpiScore, request.WeightPercent);
            assignment.UpdateScores(kpiScore, weightedScore);
        }

        await _repository.UpdateAsync(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
