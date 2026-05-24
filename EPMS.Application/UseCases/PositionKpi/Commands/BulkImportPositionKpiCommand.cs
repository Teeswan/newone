using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using EPMS.Shared.DTOs;
using MediatR;

namespace EPMS.Application.UseCases.PositionKpi.Commands;

public record BulkImportPositionKpiCommand(
    List<PositionKpiImportDto> Kpis,
    int? EmployeeId
) : IRequest<Result<BulkImportResultDto>>;

public class BulkImportPositionKpiCommandHandler
    : IRequestHandler<BulkImportPositionKpiCommand, Result<BulkImportResultDto>>
{
    private readonly IKpiRepository _kpiRepository;
    private readonly IPositionKpiRepository _positionKpiRepository;
    private readonly IAuditLogService _auditLogService;

    public BulkImportPositionKpiCommandHandler(
        IKpiRepository kpiRepository,
        IPositionKpiRepository positionKpiRepository,
        IAuditLogService auditLogService)
    {
        _kpiRepository = kpiRepository;
        _positionKpiRepository = positionKpiRepository;
        _auditLogService = auditLogService;
    }

    public async Task<Result<BulkImportResultDto>> Handle(
        BulkImportPositionKpiCommand request,
        CancellationToken cancellationToken)
    {
        var result = new BulkImportResultDto();

        foreach (var importDto in request.Kpis)
        {
            try
            {
                // Check duplicate KPI for position
                bool exists = await _positionKpiRepository.ExistsDuplicateAsync(
                    importDto.KpiName,
                    importDto.Category,
                    importDto.PositionId);

                if (exists)
                {
                    result.Rejected++;
                    result.Errors.Add(
                        $"Duplicate KPI found: {importDto.KpiName}");
                    continue;
                }

                var kpi = Domain.Entities.Kpi.Create(
                    importDto.KpiName,
                    importDto.Category,
                    importDto.Unit,
                    importDto.WeightPercent,
                    importDto.TargetValue,
                    importDto.PriorityLevel,
                    importDto.Direction,
                    request.EmployeeId);

                await _kpiRepository.CreateAsync(kpi);

                var positionKpi = Domain.Entities.PositionKpi.Create(
                    importDto.PositionId,
                    kpi.KpiId,
                    importDto.WeightPercent,
                    true);

                await _positionKpiRepository.AddAsync(positionKpi);

                result.Imported++;

                await _auditLogService.LogAsync(
                    "PositionKpi",
                    "Import",
                    positionKpi.PositionKpiId,
                    $"Imported KPI: {importDto.KpiName}",
                    request.EmployeeId);
            }
            catch (Exception ex)
            {
                result.Rejected++;

                result.Errors.Add(
                    $"Error importing {importDto.KpiName}: {ex.Message}");
            }
        }

        return Result<BulkImportResultDto>.Success(result);
    }
}