using EPMS.Domain.Enums;

namespace EPMS.Shared.DTOs;

public record PositionKpiImportDto(
    string KpiName,
    string? Category,
    string? Unit,
    decimal WeightPercent,
    decimal? TargetValue,
    PriorityLevel PriorityLevel,
    KpiDirection Direction,
    int? PositionId);

public record EmployeeKpiImportDto(
    int EmployeeId,
    int CycleId,
    string KpiName,
    string? Category,
    string? Unit,
    decimal WeightPercent,
    decimal TargetValue,
    KpiDirection Direction);
