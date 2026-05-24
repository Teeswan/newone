using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using MediatR;
using System;

namespace EPMS.Application.UseCases.PositionKpi.Commands;

public record BulkImportPositionKpiCommand(List<PositionKpiImportDto> Kpis, int? EmployeeId) : IRequest<Result<BulkImportResultDto>>;

public class BulkImportPositionKpiCommandHandler : IRequestHandler<BulkImportPositionKpiCommand, Result<BulkImportResultDto>>
{
    private readonly IPositionKpiRepository _repository;
    private readonly IAuditLogService _auditLogService;

    public BulkImportPositionKpiCommandHandler(IPositionKpiRepository repository, IAuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
    }

    public async Task<Result<BulkImportResultDto>> Handle(BulkImportPositionKpiCommand request, CancellationToken cancellationToken)
    {
        var result = new BulkImportResultDto();

        foreach (var importDto in request.Kpis)
        {
            try
            {
                if (await _repository.ExistsDuplicateAsync(importDto.KpiName, importDto.Category, importDto.PositionId))
                {
                    result.Rejected++;
                    result.Errors.Add($"Duplicate KPI found: {importDto.KpiName}");
                    continue;
                }

                var kpi = Domain.Entities.PositionKpi.Create(
                    importDto.KpiName,
                    importDto.Category,
                    importDto.Unit,
                    importDto.WeightPercent,
                    importDto.TargetValue,
                    importDto.PriorityLevel,
                    importDto.Direction,
                    importDto.PositionId,
                    request.EmployeeId);

                await _repository.AddAsync(kpi);
                result.Imported++;
                await _auditLogService.LogAsync("PositionKpi", "Import", kpi.KpiId, $"Imported KPI: {kpi.KpiName}", request.EmployeeId);
            }
            catch (Exception ex)
            {
                result.Rejected++;
                result.Errors.Add($"Error importing {importDto.KpiName}: {ex.Message}");
            }
        }

        return Result<BulkImportResultDto>.Success(result);
    }
}
