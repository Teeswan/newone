using System;
using EPMS.Domain.Enums;
using EPMS.Domain.Exceptions;

namespace EPMS.Domain.Entities;

public class PositionKpi
{
    public int PositionKpiId { get; private set; }

    public int? PositionId { get; private set; }

    public int KpiId { get; private set; }

    public decimal DefaultWeightPercent { get; private set; }

    public bool IsRequired { get; private set; }

    public virtual Position? Position { get; private set; }

    public virtual Kpi Kpi { get; private set; } = null!;


    private PositionKpi() { }

    public static PositionKpi Create(
        int? positionId,
        int kpiId,
        decimal defaultWeightPercent,
        bool isRequired)
    {
        return new PositionKpi
        {
            PositionId = positionId,
            KpiId = kpiId,
            DefaultWeightPercent = defaultWeightPercent,
            IsRequired = isRequired
        };
    }

    public void Update(
        decimal defaultWeightPercent,
        bool isRequired)
    {
        DefaultWeightPercent = defaultWeightPercent;
        IsRequired = isRequired;
    }
}
