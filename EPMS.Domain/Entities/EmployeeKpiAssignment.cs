using System;
using System.Collections.Generic;
using EPMS.Domain.Enums;
using EPMS.Domain.Events;

namespace EPMS.Domain.Entities;

public class EmployeeKpiAssignment
{
    public int AssignmentId { get; private set; }
    public int EmployeeId { get; private set; }
    public int CycleId { get; private set; }
    public int? KpiId { get; private set; } // NULL for ad-hoc
    
    // Snapshot fields
    public string KpiNameSnapshot { get; private set; } = null!;
    public string? CategorySnapshot { get; private set; }
    public string? UnitSnapshot { get; private set; }
    public KpiDirection Direction { get; private set; }
    public decimal WeightPercent { get; private set; }
    public decimal TargetValue { get; private set; }
    
    public decimal? ActualValue { get; private set; }
    public decimal? KpiScore { get; private set; }
    public decimal? WeightedScore { get; private set; }
    public int VersionNo { get; private set; } = 1;
    public KpiAssignmentStatus Status { get; private set; } = KpiAssignmentStatus.Draft;
    public bool IsAdHoc { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Domain events
    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    // Navigation properties
    public virtual Employee Employee { get; private set; } = null!;
    public virtual AppraisalCycle Cycle { get; private set; } = null!;
    public virtual KpiMaster? Kpi { get; private set; }

    private EmployeeKpiAssignment() { } // EF Core

    public static EmployeeKpiAssignment CreateSnapshot(
        int employeeId,
        int cycleId,
        KpiMaster kpi)
    {
        return new EmployeeKpiAssignment
        {
            EmployeeId = employeeId,
            CycleId = cycleId,
            KpiId = kpi.KpiId,
            KpiNameSnapshot = kpi.KpiName,
            CategorySnapshot = kpi.Category,
            UnitSnapshot = kpi.Unit,
            Direction = kpi.Direction,
            WeightPercent = kpi.WeightPercent,
            TargetValue = kpi.TargetValue ?? 0,
            IsAdHoc = false,
            CreatedAt = DateTime.UtcNow,
            Status = KpiAssignmentStatus.Draft
        };
    }

    public static EmployeeKpiAssignment CreateAdHoc(
        int employeeId,
        int cycleId,
        string name,
        string? category,
        string? unit,
        KpiDirection direction,
        decimal weight,
        decimal target)
    {
        return new EmployeeKpiAssignment
        {
            EmployeeId = employeeId,
            CycleId = cycleId,
            KpiId = null,
            KpiNameSnapshot = name,
            CategorySnapshot = category,
            UnitSnapshot = unit,
            Direction = direction,
            WeightPercent = weight,
            TargetValue = target,
            IsAdHoc = true,
            CreatedAt = DateTime.UtcNow,
            Status = KpiAssignmentStatus.Draft
        };
    }

    public void SetActualValue(decimal actual)
    {
        if (Status != KpiAssignmentStatus.Active)
            throw new InvalidOperationException("Cannot enter actual value unless assignment is Active.");

        ActualValue = actual;
        _domainEvents.Add(new KpiActualEnteredEvent(AssignmentId, actual));
    }

    public void UpdateScores(decimal kpiScore, decimal weightedScore)
    {
        KpiScore = kpiScore;
        WeightedScore = weightedScore;
    }

    public void Activate()
    {
        Status = KpiAssignmentStatus.Active;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
