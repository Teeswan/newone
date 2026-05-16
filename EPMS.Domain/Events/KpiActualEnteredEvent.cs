using MediatR;

namespace EPMS.Domain.Events;

public class KpiActualEnteredEvent : INotification
{
    public int AssignmentId { get; }
    public decimal ActualValue { get; }

    public KpiActualEnteredEvent(int assignmentId, decimal actualValue)
    {
        AssignmentId = assignmentId;
        ActualValue = actualValue;
    }
}
