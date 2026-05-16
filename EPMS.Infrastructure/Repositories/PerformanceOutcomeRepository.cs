using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static EPMS.Infrastructure.StoredProcedures;

namespace EPMS.Infrastructure.Repositories;

public class PerformanceOutcomeRepository : BaseRepository<PerformanceOutcome, int>, IPerformanceOutcomeRepository
{
    private readonly ISqlRepository<PerformanceOutcome, int> _sqlRepository;

    public PerformanceOutcomeRepository(AppDbContext context, ISqlRepository<PerformanceOutcome, int> sqlRepository) : base(context)
    {
        _sqlRepository = sqlRepository ?? throw new ArgumentNullException(nameof(sqlRepository));
    }

    public override async Task<IEnumerable<PerformanceOutcome>> GetAllAsync()
    {
        return await _sqlRepository.FromSqlAsync(PerformanceOutcomes_GetAll);
    }

    public override async Task<PerformanceOutcome?> GetByIdAsync(int outcomeId)
    {
        return await _sqlRepository.FromSqlFirstOrDefaultAsync(PerformanceOutcomes_GetById, new SqlParameter("@OutcomeID", outcomeId));
    }

    public override async Task<PerformanceOutcome> CreateAsync(PerformanceOutcome entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@EvalID", (object?)entity.EvalId ?? DBNull.Value),
            new SqlParameter("@EmployeeID", (object?)entity.EmployeeId ?? DBNull.Value),
            new SqlParameter("@CycleID", (object?)entity.CycleId ?? DBNull.Value),
            new SqlParameter("@RecommendationType", (object?)entity.RecommendationType ?? DBNull.Value),
            new SqlParameter("@OldPositionId", (object?)entity.OldPositionId ?? DBNull.Value),
            new SqlParameter("@NewPositionId", (object?)entity.NewPositionId ?? DBNull.Value),
            new SqlParameter("@OldLevelId", (object?)entity.OldLevelId ?? DBNull.Value),
            new SqlParameter("@NewLevelId", (object?)entity.NewLevelId ?? DBNull.Value),
            new SqlParameter("@ApprovalStatus", (object?)entity.ApprovalStatus ?? DBNull.Value),
            new SqlParameter("@EffectiveDate", (object?)entity.EffectiveDate ?? DBNull.Value)
        };

        var result = await _sqlRepository.FromSqlFirstOrDefaultAsync(PerformanceOutcomes_Create, parameters);
        return result ?? throw new InvalidOperationException("Failed to create performance outcome.");
    }

    public override async Task<PerformanceOutcome?> UpdateAsync(PerformanceOutcome entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@OutcomeID", entity.OutcomeId),
            new SqlParameter("@EvalID", (object?)entity.EvalId ?? DBNull.Value),
            new SqlParameter("@EmployeeID", (object?)entity.EmployeeId ?? DBNull.Value),
            new SqlParameter("@CycleID", (object?)entity.CycleId ?? DBNull.Value),
            new SqlParameter("@RecommendationType", (object?)entity.RecommendationType ?? DBNull.Value),
            new SqlParameter("@OldPositionId", (object?)entity.OldPositionId ?? DBNull.Value),
            new SqlParameter("@NewPositionId", (object?)entity.NewPositionId ?? DBNull.Value),
            new SqlParameter("@OldLevelId", (object?)entity.OldLevelId ?? DBNull.Value),
            new SqlParameter("@NewLevelId", (object?)entity.NewLevelId ?? DBNull.Value),
            new SqlParameter("@ApprovalStatus", (object?)entity.ApprovalStatus ?? DBNull.Value),
            new SqlParameter("@EffectiveDate", (object?)entity.EffectiveDate ?? DBNull.Value)
        };

        return await _sqlRepository.FromSqlFirstOrDefaultAsync(PerformanceOutcomes_Update, parameters);
    }

    public override async Task<bool> DeleteAsync(int outcomeId)
    {
        var rowsAffected = await _sqlRepository.ExecuteSqlAsync(PerformanceOutcomes_Delete, new SqlParameter("@OutcomeID", outcomeId));
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<PerformanceOutcome>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _sqlRepository.FromSqlAsync(PerformanceOutcomes_GetByEmployeeId, new SqlParameter("@EmployeeID", employeeId));
    }

    public async Task<IEnumerable<PerformanceOutcome>> GetByCycleIdAsync(int cycleId)
    {
        return await _sqlRepository.FromSqlAsync(PerformanceOutcomes_GetByCycleId, new SqlParameter("@CycleID", cycleId));
    }
}
