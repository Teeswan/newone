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

namespace EPMS.Application.UseCases.KpiMaster.Commands;

public record BulkImportKpiMasterCommand(List<KpiImportDto> Kpis, int? UserId) : IRequest<Result<BulkImportResultDto>>;

public record KpiImportDto(
    string KpiName,
    string? Category,
    string? Unit,
    decimal WeightPercent,
    decimal? TargetValue,
    PriorityLevel PriorityLevel,
    KpiDirection Direction,
    int? PositionId);

public class BulkImportKpiMasterCommandHandler : IRequestHandler<BulkImportKpiMasterCommand, Result<BulkImportResultDto>>
{
    private readonly IKpiMasterRepository _repository;
    private readonly IAuditLogService _auditLogService;

    public BulkImportKpiMasterCommandHandler(IKpiMasterRepository repository, IAuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
    }

    public async Task<Result<BulkImportResultDto>> Handle(BulkImportKpiMasterCommand request, CancellationToken cancellationToken)
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

                var kpi = Domain.Entities.KpiMaster.Create(
                    importDto.KpiName,
                    importDto.Category,
                    importDto.Unit,
                    importDto.WeightPercent,
                    importDto.TargetValue,
                    importDto.PriorityLevel,
                    importDto.Direction,
                    importDto.PositionId,
                    request.UserId);

                await _repository.AddAsync(kpi);
                result.Imported++;
                await _auditLogService.LogAsync("KpiMaster", "Import", kpi.KpiId, $"Imported KPI: {kpi.KpiName}", request.UserId);
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
