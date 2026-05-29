using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Commands;

public record AssignKpiFromMasterCommand(int EmployeeId, int CycleId, int KpiId) : IRequest<Result<int>>;

public class AssignKpiFromMasterCommandHandler : IRequestHandler<AssignKpiFromMasterCommand, Result<int>>
{
    private readonly IKpiAssignmentRepository _repository;
    private readonly IPositionKpiRepository _masterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignKpiFromMasterCommandHandler(
        IKpiAssignmentRepository repository,
        IPositionKpiRepository masterRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _masterRepository = masterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(AssignKpiFromMasterCommand request, CancellationToken cancellationToken)
    {
        var masterKpi = await _masterRepository.GetByIdAsync(request.KpiId);
        if (masterKpi == null) return Result<int>.Failure("Position KPI template not found.");

        var existing = await _repository.GetSnapshotAsync(request.EmployeeId, request.CycleId, request.KpiId);
        if (existing != null) return Result<int>.Failure("KPI already assigned to this employee for this cycle.");

        var assignment = EmployeeKpi.CreateSnapshot(request.EmployeeId, request.CycleId, masterKpi);
        
        await _repository.AddAsync(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(assignment.EmployeeKpiId);
    }
}
