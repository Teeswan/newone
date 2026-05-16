using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Data.SqlClient;
using static EPMS.Infrastructure.StoredProcedures;

namespace EPMS.Infrastructure.Repositories;

/// <summary>
/// Repository for appraisal cycles, implemented with stored procedure support.
/// </summary>
public class AppraisalCycleRepository : BaseRepository<AppraisalCycle, int>, IAppraisalCycleRepository
{
    private readonly ISqlRepository<AppraisalCycle, int> _sqlRepository;

    public AppraisalCycleRepository(AppDbContext context, ISqlRepository<AppraisalCycle, int> sqlRepository) : base(context)
    {
        _sqlRepository = sqlRepository ?? throw new ArgumentNullException(nameof(sqlRepository));
    }

    public override Task<IEnumerable<AppraisalCycle>> GetAllAsync()
    {
        return _sqlRepository.FromSqlAsync(AppraisalCycles_GetAll);
    }

    public override Task<AppraisalCycle?> GetByIdAsync(int cycleId)
    {
        return _sqlRepository.FromSqlFirstOrDefaultAsync(AppraisalCycles_GetById, new SqlParameter("@CycleID", cycleId));
    }

    public override async Task<AppraisalCycle> CreateAsync(AppraisalCycle entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@CycleName", entity.CycleName),
            new SqlParameter("@StartDate", entity.StartDate),
            new SqlParameter("@EndDate", entity.EndDate),
            new SqlParameter("@EvaluationPeriod", ToDbValue(entity.EvaluationPeriod)),
            new SqlParameter("@CycleStatus", ToDbValue(entity.CycleStatus))
        };

        var result = await _sqlRepository.FromSqlFirstOrDefaultAsync(AppraisalCycles_Create, parameters);
        return result ?? throw new InvalidOperationException("Failed to create appraisal cycle record.");
    }

    public override Task<AppraisalCycle?> UpdateAsync(AppraisalCycle entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var parameters = new object[]
        {
            new SqlParameter("@CycleID", entity.CycleId),
            new SqlParameter("@CycleName", entity.CycleName),
            new SqlParameter("@StartDate", entity.StartDate),
            new SqlParameter("@EndDate", entity.EndDate),
            new SqlParameter("@EvaluationPeriod", ToDbValue(entity.EvaluationPeriod)),
            new SqlParameter("@CycleStatus", ToDbValue(entity.CycleStatus))
        };

        return _sqlRepository.FromSqlFirstOrDefaultAsync(AppraisalCycles_Update, parameters);
    }

    public override async Task<bool> DeleteAsync(int cycleId)
    {
        var rowsAffected = await _sqlRepository.ExecuteSqlAsync(AppraisalCycles_Delete, new SqlParameter("@CycleID", cycleId));
        return rowsAffected > 0;
    }

    private static object ToDbValue<T>(T? value)
        => value is null ? DBNull.Value : value!;
}
