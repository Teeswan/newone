using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using MediatR;

namespace EPMS.Application.UseCases.KpiAssignment.Commands;

public record BulkImportEmployeeKpiCommand(List<EmployeeKpiImportDto> Kpis, int? UserId) : IRequest<Result<BulkImportResultDto>>;

public record EmployeeKpiImportDto(
    int EmployeeId,
    int CycleId,
    string KpiName,
    string? Category,
    string? Unit,
    decimal WeightPercent,
    decimal TargetValue,
    KpiDirection Direction);

public class BulkImportEmployeeKpiCommandHandler : IRequestHandler<BulkImportEmployeeKpiCommand, Result<BulkImportResultDto>>
{
    private readonly IKpiAssignmentRepository _repository;
    private readonly IAuditLogService _auditLogService;

    public BulkImportEmployeeKpiCommandHandler(IKpiAssignmentRepository repository, IAuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
    }

    public async Task<Result<BulkImportResultDto>> Handle(BulkImportEmployeeKpiCommand request, CancellationToken cancellationToken)
    {
        var result = new BulkImportResultDto();

        foreach (var importDto in request.Kpis)
        {
            try
            {
                var adHocKpi = EmployeeKpiAssignment.CreateAdHoc(
                    importDto.EmployeeId,
                    importDto.CycleId,
                    importDto.KpiName,
                    importDto.Category,
                    importDto.Unit,
                    importDto.Direction,
                    importDto.WeightPercent,
                    importDto.TargetValue);

                await _repository.AddAsync(adHocKpi);
                result.Imported++;
                await _auditLogService.LogAsync("EmployeeKpiAssignment", "Import", adHocKpi.AssignmentId, $"Imported Ad-hoc KPI for Employee {importDto.EmployeeId}: {importDto.KpiName}", request.UserId);
            }
            catch (System.Exception ex)
            {
                result.Rejected++;
                result.Errors.Add($"Error importing KPI for Employee {importDto.EmployeeId}: {ex.Message}");
            }
        }

        return Result<BulkImportResultDto>.Success(result);
    }
}
