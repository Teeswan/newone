using EPMS.Shared.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EPMS.Shared.DTOs;
using EPMS.Application.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.DataAccess;

public class KpiQueryService : IKpiQueryService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly AppDbContext _context;

    public KpiQueryService(IDbConnectionFactory connectionFactory, AppDbContext context)
    {
        _connectionFactory = connectionFactory;
        _context = context;
    }

    public async Task<IEnumerable<PositionKpiDto>> GetKpiByPositionAsync(int? positionId, bool isActive = true, bool globalOnly = false)
    {
        using var connection = _connectionFactory.CreateConnection();

        var whereClause = globalOnly 
            ? "WHERE pk.PositionID IS NULL" 
            : "WHERE (@PositionId IS NULL OR pk.PositionID = @PositionId)";

        var sql = $@"
        SELECT 
            pk.PositionKPIID as PositionKpiId,
            pk.KpiID as KpiId,
            k.KPIName as KpiName,
            k.Category as Category,
            k.Unit as Unit,
            pk.DefaultWeightPercent as WeightPercent,
            pk.TargetValue as TargetValue,
            k.Direction as Direction,
            pk.PositionID as PositionId,
            p.PositionTitle as PositionName,
            pk.IsRequired as IsRequired,
            k.IsActive as IsKpiActive,
            pk.IsActive as IsPositionKpiActive
        FROM PositionKPIs pk
        INNER JOIN KPIs k ON pk.KpiID = k.KpiID
        LEFT JOIN Positions p ON pk.PositionID = p.PositionID
        {whereClause}
          AND k.IsActive = @IsActive
          AND pk.IsActive = 1
        ORDER BY k.KPIName";

        return await connection.QueryAsync<PositionKpiDto>(sql, new { PositionId = positionId, IsActive = isActive });
    }

    public async Task<PaginatedResult<PositionKpiDto>> GetPagedPositionKpiAsync(int? positionId, bool isActive, int pageNumber, int pageSize, string? searchTerm = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var offset = (pageNumber - 1) * pageSize;
        var searchFilter = string.IsNullOrWhiteSpace(searchTerm) ? "" : "AND (k.KPIName LIKE @SearchTerm OR k.Category LIKE @SearchTerm OR p.PositionTitle LIKE @SearchTerm)";

        var sql = $@"
        SELECT 
            pk.PositionKPIID as PositionKpiId,
            pk.KpiID as KpiId,
            k.KPIName as KpiName,
            k.Category as Category,
            k.Unit as Unit,
            pk.DefaultWeightPercent as WeightPercent,
            pk.TargetValue as TargetValue,
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
          {searchFilter}
        ORDER BY k.KPIName
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1)
        FROM PositionKPIs pk
        INNER JOIN KPIs k ON pk.KpiID = k.KpiID
        LEFT JOIN Positions p ON pk.PositionID = p.PositionID
        WHERE (@PositionId IS NULL OR pk.PositionID = @PositionId)
          AND k.IsActive = @IsActive
          AND pk.IsActive = 1
          {searchFilter};";

        using var multi = await connection.QueryMultipleAsync(sql, new 
        { 
            PositionId = positionId, 
            IsActive = isActive, 
            SearchTerm = $"%{searchTerm}%",
            Offset = offset,
            PageSize = pageSize 
        });

        var items = await multi.ReadAsync<PositionKpiDto>();
        var totalCount = await multi.ReadFirstAsync<int>();

        return new PaginatedResult<PositionKpiDto>(items, totalCount, pageNumber, pageSize);
    }


    public async Task<IEnumerable<AggregatedKpiDto>> GetDepartmentKpiSummaryAsync(int cycleId)
    {
        // Pure C# Implementation using LINQ
        var summary = await _context.Departments
            .Where(d => !d.IsDeleted)
            .Select(d => new AggregatedKpiDto
            {
                EntityId = d.DepartmentId,
                EntityName = d.DepartmentName,
                CycleId = cycleId,
                EmployeeCount = d.Employees.Count(e => !e.IsDeleted),
                AverageScore = d.Teams
                    .SelectMany(t => t.Employees)
                    .SelectMany(e => e.EmployeeKpis)
                    .Where(ek => ek.CycleId == cycleId)
                    .Average(ek => (decimal?)ek.Score) ?? 0
            })
            .ToListAsync();

        return summary;
    }

    public async Task<IEnumerable<AggregatedKpiDto>> GetTeamKpiSummaryAsync(int cycleId)
    {
        // Pure C# Implementation using LINQ
        var summary = await _context.Teams
            .Where(t => !t.IsDeleted)
            .Select(t => new AggregatedKpiDto
            {
                EntityId = t.TeamId,
                EntityName = t.TeamName,
                CycleId = cycleId,
                EmployeeCount = t.Employees.Count(e => !e.IsDeleted),
                AverageScore = t.Employees
                    .SelectMany(e => e.EmployeeKpis)
                    .Where(ek => ek.CycleId == cycleId)
                    .Average(ek => (decimal?)ek.Score) ?? 0
            })
            .ToListAsync();

        return summary;
    }

    public async Task<IEnumerable<AggregatedKpiDto>> GetPositionKpiSummaryAsync(int cycleId)
    {
        // Pure C# Implementation using LINQ
        var summary = await _context.Positions
            .Where(p => !p.IsDeleted)
            .Select(p => new AggregatedKpiDto
            {
                EntityId = p.PositionId,
                EntityName = p.PositionTitle,
                CycleId = cycleId,
                EmployeeCount = p.Employees.Count(e => !e.IsDeleted),
                AverageScore = p.Employees
                    .SelectMany(e => e.EmployeeKpis)
                    .Where(ek => ek.CycleId == cycleId)
                    .Average(ek => (decimal?)ek.Score) ?? 0
            })
            .ToListAsync();

        return summary;
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
