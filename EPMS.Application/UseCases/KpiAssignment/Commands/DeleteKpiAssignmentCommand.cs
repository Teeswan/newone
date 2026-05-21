using EPMS.Domain.Enums;
using EPMS.Shared.Common;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Commands;

public record DeleteKpiAssignmentCommand(int AssignmentId) : IRequest<Result<bool>>;

public class DeleteKpiAssignmentCommandHandler : IRequestHandler<DeleteKpiAssignmentCommand, Result<bool>>
{
    private readonly EPMS.Domain.Interfaces.IKpiAssignmentRepository _repository;
    private readonly EPMS.Domain.Interfaces.IUnitOfWork _unitOfWork;

    public DeleteKpiAssignmentCommandHandler(
        EPMS.Domain.Interfaces.IKpiAssignmentRepository repository,
        EPMS.Domain.Interfaces.IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteKpiAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetByIdAsync(request.AssignmentId);
        if (assignment == null)
            return Result<bool>.Failure("Assignment not found.");

        if (assignment.Status == KpiAssignmentStatus.Locked)
            return Result<bool>.Failure("Cannot delete a locked assignment.");

        await _repository.DeleteAsync(request.AssignmentId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
