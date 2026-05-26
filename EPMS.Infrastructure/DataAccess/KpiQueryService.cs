using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Infrastructure.DataAccess;

namespace EPMS.Infrastructure.DataAccess;

public class KpiQueryService : IKpiQueryService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public KpiQueryService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<PositionKpiDto>> GetKpiByPositionAsync(int? positionId, bool isActive = true)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
        SELECT 
            pk.PositionKPIID as PositionKpiId,
            pk.KpiID as KpiId,
            k.KPIName as KpiName,
            k.Category as Category,
            k.Unit as Unit,
            pk.DefaultWeightPercent as WeightPercent,
            k.Direction as Direction,
            pk.PositionID as PositionId,
            p.PositionTitle as PositionName,
            pk.IsRequired as IsRequired,
            k.IsActive as IsKpiActive,
            pk.IsActive as IsPositionKpiActive
        FROM PositionKPIs pk
        INNER JOIN KPIs k ON pk.KpiID = k.KpiID
        LEFT JOIN Positions p ON pk.PositionID = p.PositionID
        WHERE (@PositionId IS NULL OR pk.PositionID = @PositionId)
          AND k.IsActive = @IsActive
          AND pk.IsActive = 1
        ORDER BY k.KPIName";

        return await connection.QueryAsync<PositionKpiDto>(
            sql,
            new { PositionId = positionId, IsActive = isActive });
    }

    public async Task<IEnumerable<EmployeeKpiAssignmentDto>> GetEmployeeKpiAssignmentAsync(int employeeId, int cycleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<EmployeeKpiAssignmentDto>(
            "sp_GetEmployeeKpiAssignment",
            new { EmployeeId = employeeId, CycleId = cycleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<KpiScoreSummaryDto?> GetKpiScoreSummaryAsync(int employeeId, int cycleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<KpiScoreSummaryDto>(
            "sp_GetKpiScoreSummary",
            new { EmployeeId = employeeId, CycleId = cycleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<AggregatedKpiDto>> GetDepartmentKpiSummaryAsync(int cycleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<AggregatedKpiDto>(
            "sp_GetDepartmentKpiSummary",
            new { CycleId = cycleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<AggregatedKpiDto>> GetTeamKpiSummaryAsync(int cycleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<AggregatedKpiDto>(
            "sp_GetTeamKpiSummary",
            new { CycleId = cycleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<AggregatedKpiDto>> GetPositionKpiSummaryAsync(int cycleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<AggregatedKpiDto>(
            "sp_GetPositionKpiSummary",
            new { CycleId = cycleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<dynamic>> GetKpiAuditLogAsync(string entityType, DateTime? dateFrom, DateTime? dateTo, int? userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync(
            "sp_GetKpiAuditLog",
            new { EntityType = entityType, DateFrom = dateFrom, DateTo = dateTo, UserId = userId },
            commandType: CommandType.StoredProcedure);
    }
}
