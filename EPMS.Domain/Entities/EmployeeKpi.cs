using System;
using System.Collections.Generic;
using EPMS.Domain.Enums;
using EPMS.Domain.Events;

namespace EPMS.Domain.Entities;

public class EmployeeKpi
{
    public int EmployeeKpiId { get; private set; }
    public int EmployeeId { get; private set; }
    public int CycleId { get; private set; }
    public int KpiId { get; private set; }      
    public int TeamKpiId { get; private set; }    

    // Snapshot fields
    public string KpiNameSnapshot { get; private set; } = null!;
    public string? CategorySnapshot { get; private set; }
    public string? UnitSnapshot { get; private set; }
    public KpiDirection Direction { get; private set; }

    // KPI values
    public decimal WeightPercent { get; private set; }
    public decimal TargetValue { get; private set; }

    // Score fields
    public decimal? ActualValue { get; private set; }
    public decimal? KpiScore { get; private set; }
    public decimal? WeightedScore { get; private set; }

    // Status
    public bool IsActive { get; private set; }
    public KpiAssignmentStatus Status { get; private set; } = KpiAssignmentStatus.Draft;
    public bool IsAdHoc { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int VersionNo { get; private set; } = 1;

    // Domain events
    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    // Navigation properties
    public virtual Employee Employee { get; private set; } = null!;
    public virtual AppraisalCycle Cycle { get; private set; } = null!;
    public virtual PositionKpi? Kpi { get; private set; }
    public virtual TeamKpi? TeamKpi { get; private set; }

    private EmployeeKpi() { } // EF Core

    public static EmployeeKpi Create(
        int employeeId,
        int cycleId,
        int? teamKpiId,
        decimal targetValue,
        decimal weightPercent,
        bool isActive,
        decimal? actualValue,
        KpiDirection direction,
        string kpiName,
        string? category = null,
        string? unit = null)
    {
        var kpi = new EmployeeKpi
        {
            EmployeeId = employeeId,
            CycleId = cycleId,
            TeamKpiId = teamKpiId,
            TargetValue = targetValue,
            WeightPercent = weightPercent,
            IsActive = isActive,
            ActualValue = actualValue,
            Direction = direction,
            KpiNameSnapshot = kpiName,
            CategorySnapshot = category,
            UnitSnapshot = unit,
            CreatedAt = DateTime.UtcNow,
            Status = KpiAssignmentStatus.Active,
            IsAdHoc = false
        };
        kpi.CalculateScores();
        return kpi;
    }

    public static EmployeeKpi CreateSnapshot(
        int employeeId,
        int cycleId,
        PositionKpi kpi,
        int? teamKpiId = null)
    {
        return new EmployeeKpi
        {
            EmployeeId = employeeId,
            CycleId = cycleId,
            KpiId = kpi.PositionKpiId,
            TeamKpiId = teamKpiId,
            KpiNameSnapshot = kpi.Kpi.KpiName,
            CategorySnapshot = kpi.Kpi.Category,
            UnitSnapshot = kpi.Kpi.Unit,
            Direction = kpi.Kpi.Direction,
            WeightPercent = kpi.DefaultWeightPercent,
            TargetValue = 0,
            IsAdHoc = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Status = KpiAssignmentStatus.Draft
        };
    }

    public static EmployeeKpi CreateAdHoc(
        int employeeId,
        int cycleId,
        string name,
        string? category,
        string? unit,
        KpiDirection direction,
        decimal weight,
        decimal target)
    {
        return new EmployeeKpi
        {
            EmployeeId = employeeId,
            CycleId = cycleId,
            KpiId = KpiId,
            TeamKpiId = null,
            KpiNameSnapshot = name,
            CategorySnapshot = category,
            UnitSnapshot = unit,
            Direction = direction,
            WeightPercent = weight,
            TargetValue = target,
            IsAdHoc = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Status = KpiAssignmentStatus.Draft
        };
    }

    public void SetActualValue(decimal actual)
    {
        if (Status != KpiAssignmentStatus.Active)
            throw new InvalidOperationException("Cannot enter actual value unless assignment is Active.");

        ActualValue = actual;
        CalculateScores();
        _domainEvents.Add(new KpiActualEnteredEvent(EmployeeKpiId, actual));
    }

    public void Update(
        decimal targetValue,
        decimal weightPercent,
        bool isActive,
        decimal? actualValue,
        KpiDirection direction)
    {
        if (Status == KpiAssignmentStatus.Locked)
            throw new InvalidOperationException("Cannot update a locked assignment.");

        TargetValue = targetValue;
        WeightPercent = weightPercent;
        IsActive = isActive;
        ActualValue = actualValue;
        Direction = direction;

        CalculateScores();
        VersionNo++;
    }

    public void UpdateDetails(
        string name,
        string? category,
        string? unit,
        KpiDirection direction,
        decimal weight,
        decimal target)
    {
        if (Status == KpiAssignmentStatus.Locked)
            throw new InvalidOperationException("Cannot update a locked assignment.");

        KpiNameSnapshot = name;
        CategorySnapshot = category;
        UnitSnapshot = unit;
        Direction = direction;
        WeightPercent = weight;
        TargetValue = target;

        CalculateScores(); // Recalculate with new target/weight if actual exists
        VersionNo++;
    }

    public void Activate()
    {
        Status = KpiAssignmentStatus.Active;
        IsActive = true;
    }

    public void Deactivate()
    {
        Status = KpiAssignmentStatus.Locked;
        IsActive = false;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    private void CalculateScores()
    {
        if (!ActualValue.HasValue || TargetValue == 0)
        {
            KpiScore = null;
            WeightedScore = null;
            return;
        }

        if (Direction == KpiDirection.HigherIsBetter)
        {
            KpiScore = Math.Round((ActualValue.Value / TargetValue) * 100, 4);
        }
        else
        {
            if (ActualValue.Value == 0)
                KpiScore = 100;
            else
            {
                var score = (TargetValue / ActualValue.Value) * 100;
                KpiScore = Math.Round(score > 100 ? 100 : score, 4);
            }
        }

        WeightedScore = Math.Round(KpiScore.Value * (WeightPercent / 100), 4);
    }
}
