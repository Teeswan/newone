using System.Threading;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using FluentValidation;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Commands;

public record AddAdHocKpiCommand : IRequest<Result<decimal>>
{
    public int EmployeeId { get; init; }
    public int CycleId { get; init; }
    public string KpiName { get; init; } = null!;
    public string? Category { get; init; }
    public string? Unit { get; init; }
    public KpiDirection Direction { get; init; }
    public decimal WeightPercent { get; init; }
    public decimal TargetValue { get; init; }

    public class Validator : AbstractValidator<AddAdHocKpiCommand>
    {
        public Validator()
        {
            RuleFor(x => x.KpiName).NotEmpty();
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.WeightPercent).GreaterThan(0);
            RuleFor(x => x.TargetValue).GreaterThan(0);
        }
    }
}

public class AddAdHocKpiCommandHandler : IRequestHandler<AddAdHocKpiCommand, Result<decimal>>
{
    private readonly IKpiAssignmentRepository _repository;

    public AddAdHocKpiCommandHandler(IKpiAssignmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<decimal>> Handle(AddAdHocKpiCommand request, CancellationToken cancellationToken)
    {
        var adHocKpi = EmployeeKpiAssignment.CreateAdHoc(
            request.EmployeeId,
            request.CycleId,
            request.KpiName,
            request.Category,
            request.Unit,
            request.Direction,
            request.WeightPercent,
            request.TargetValue);

        await _repository.AddAsync(adHocKpi);

        var allAssignments = await _repository.GetAssignmentsByEmployeeCycleAsync(request.EmployeeId, request.CycleId);
        var totalWeight = allAssignments.Sum(a => a.WeightPercent);

        return Result<decimal>.Success(totalWeight);
    }
}
