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

    public async Task<IEnumerable<KpiMasterDto>> GetKpisByPositionAsync(int? positionId, bool isActive = true)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<KpiMasterDto>(
            "sp_GetKpisByPosition",
            new { PositionId = positionId, IsActive = isActive },
            commandType: CommandType.StoredProcedure);
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

    public async Task<IEnumerable<dynamic>> GetKpiAuditLogAsync(string entityType, DateTime? dateFrom, DateTime? dateTo, int? userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync(
            "sp_GetKpiAuditLog",
            new { EntityType = entityType, DateFrom = dateFrom, DateTo = dateTo, UserId = userId },
            commandType: CommandType.StoredProcedure);
    }
}
