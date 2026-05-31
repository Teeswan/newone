using System;
using EPMS.Domain.Enums;

namespace EPMS.Domain.Entities;

public class PositionKpi
{
    public int PositionKpiId { get; private set; }

    public int? PositionId { get; private set; }

    public int KpiId { get; private set; }

    public decimal DefaultWeightPercent { get; private set; }

    public bool IsRequired { get; private set; }

    public bool IsActive { get; private set; }

    public decimal TargetValue { get; set; }

    public virtual Position? Position { get; private set; }

    public virtual Kpi Kpi { get; private set; } = null!;

    public virtual ICollection<EmployeeKpi> EmployeeKpis { get; set; } = new List<EmployeeKpi>();

    private PositionKpi() { }

    public static PositionKpi Create(
        int? positionId,
        int kpiId,
        decimal defaultWeightPercent,
        bool isRequired,
        decimal targetValue)
    {
        return new PositionKpi
        {
            PositionId = positionId,
            KpiId = kpiId,
            DefaultWeightPercent = defaultWeightPercent,
            IsRequired = isRequired,
            TargetValue = targetValue,
            IsActive = true
        };
    }

    public void Update(
        decimal defaultWeightPercent,
        bool isRequired,
        decimal targetValue)
    {
        DefaultWeightPercent = defaultWeightPercent;
        IsRequired = isRequired;
        TargetValue = targetValue;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
